// COPYRIGHT 1995-2023 ESRI
// TRADE SECRETS: ESRI PROPRIETARY AND CONFIDENTIAL
// Unpublished material - all rights reserved under the
// Copyright Laws of the United States and applicable international
// laws, treaties, and conventions.
//
// For additional information, contact:
// Attn: Contracts and Legal Department
// Environmental Systems Research Institute, Inc.
// 380 New York Street
// Redlands, California 92373
// USA
//
// email: legal@esri.com
using Esri.ArcGISMapsSDK.Components;
using Esri.ArcGISMapsSDK.Utils;
using Esri.GameEngine.Geometry;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Samples.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[AddComponentMenu("ArcGIS Maps SDK/Samples/ArcGIS Tabletop Controller")]
	public class ArcGISTabletopControllerComponent : MonoBehaviour
	{
		public Transform TransformWrapper;
		public ArcGISMapComponent MapComponent;
		public ArcGISLocationComponent CameraComponent;

		private ArcGISPoint lastCenter;
		private double? lastElevationOffset;
		private double? lastHeight;
		private MapExtentShapes? lastShape;
		private double? lastWidth;

		[SerializeField]
		[OnChangedCall("OnCenterChanged")]
		private ArcGISPoint center = new ArcGISPoint(0, 0, 0, ArcGISSpatialReference.WGS84());
		public ArcGISPoint Center
		{
			get => center;
			set
			{
				if (center != value)
				{
					center = value;
					OnCenterChanged();
				}
			}
		}

		[SerializeField]
		[OnChangedCall("OnElevationOffsetChanged")]
		private double elevationOffset;
		public double ElevationOffset
		{
			get => elevationOffset;
			set
			{
				if (elevationOffset != value)
				{
					elevationOffset = value;
					OnElevationOffsetChanged();
				}
			}
		}

		[SerializeField]
		[OnChangedCall("OnWidthChanged")]
		private double width;
		public double Width
		{
			get => width;
			set
			{
				if (width != value)
				{
					width = value;
					OnWidthChanged();
				}
			}
		}

		[SerializeField]
		[OnChangedCall("OnHeightChanged")]
		private double height;
		public double Height
		{
			get => height;
			set
			{
				if (height != value)
				{
					height = value;
					OnHeightChanged();
				}
			}
		}

		[SerializeField]
		[OnChangedCall("OnShapeChanged")]
		private MapExtentShapes shape;
		public MapExtentShapes Shape
		{
			get => shape;
			set
			{
				if (shape != value)
				{
					shape = value;
					OnShapeChanged();
				}
			}
		}

		internal void OnCenterChanged()
		{
			PreUpdateTabletop();
		}

		internal void OnHeightChanged()
		{
			PreUpdateTabletop();
		}

		internal void OnShapeChanged()
		{
			PreUpdateTabletop();
		}

		internal void OnWidthChanged()
		{
			PreUpdateTabletop();
		}

		private void OnDisable()
		{
			MapComponent.ExtentUpdated -= new ArcGISExtentUpdatedEventHandler(PostUpdateTabletop);
		}

		internal void OnElevationOffsetChanged()
		{
			PreUpdateTabletop();
		}

		private void OnEnable()
		{
			MapComponent.ExtentUpdated += new ArcGISExtentUpdatedEventHandler(PostUpdateTabletop);

			lastCenter = null;
			lastElevationOffset = null;
			lastWidth = null;
			lastHeight = null;
			lastShape = null;

			PreUpdateTabletop();
		}

		private void PostUpdateTabletop(ArcGISExtentUpdatedEventArgs e)
		{
			if (!e.Type.HasValue)
			{
				return;
			}

			var areaMin = e.AreaMin.Value;
			var areaMax = e.AreaMax.Value;

			// Adjust center and scale only after all tiles were updated
			var width = areaMax.x - areaMin.x;
			var altitude = areaMax.y - areaMin.y;
			var height = areaMax.z - areaMin.z;
			var centerPosition = new double3(areaMin.x + width / 2.0, areaMin.y + altitude, areaMin.z + height / 2.0);

			MapComponent.OriginPosition = MapComponent.View.WorldToGeographic(centerPosition);

			var scale = 1.0 / math.max(width, height);

			TransformWrapper.localScale = new Vector3((float)scale, (float)scale, (float)scale);

			UpdateOffset();

			// We need to force an HP Root update to account for scale changes
			MapComponent.UpdateHPRoot();
		}

		private void PreUpdateTabletop()
		{
			bool needsOffsetUpdate = lastElevationOffset != ElevationOffset;
			bool needsExtentUpdate = (lastCenter != Center) ||
				(lastWidth != Width) ||
				(lastHeight != Height && Shape == MapExtentShapes.Rectangle) ||
				(lastShape != Shape);

			if (!needsExtentUpdate && !needsOffsetUpdate)
			{
				return;
			}

			if (needsExtentUpdate)
			{
				double2 shapeDimensions = Shape == MapExtentShapes.Rectangle ? new double2(Width, Height) : new double2(Width, Width);

				var newExtent = new ArcGISExtentInstanceData()
				{
					GeographicCenter = (ArcGISPoint)Center.Clone(),
					ExtentShape = Shape,
					ShapeDimensions = shapeDimensions,
				};

				// Camera height should be radius to the furthest point. For square or rectangle, we use diagonal distance to corner.
				double radiusDistance = 0.0;
				if (Shape == MapExtentShapes.Circle)
				{
					radiusDistance = Width;
				}
				else if (Shape == MapExtentShapes.Square)
				{
					radiusDistance = (Width / 2) * math.sqrt(2);
				}
				else if (Shape == MapExtentShapes.Rectangle)
				{
					radiusDistance = math.sqrt(math.pow(Width / 2, 2) + math.pow(Height / 2, 2));
				}

				if (MapComponent.Extent == newExtent)
				{
					MapComponent.OriginPosition = newExtent.GeographicCenter;

					var scale = 1.0 / (2 * radiusDistance);

					TransformWrapper.localScale = new Vector3((float)scale, (float)scale, (float)scale);
				}
				else
				{
					MapComponent.Extent = new ArcGISExtentInstanceData()
					{
						GeographicCenter = (ArcGISPoint)Center.Clone(),
						ExtentShape = Shape,
						ShapeDimensions = shapeDimensions,
					};
				}

				CameraComponent.Position = new ArcGISPoint(Center.X, Center.Y, radiusDistance, Center.SpatialReference);

				lastCenter = (ArcGISPoint)Center.Clone();
				lastWidth = Width;
				lastHeight = Height;
				lastShape = Shape;
			}

			if (needsOffsetUpdate)
			{
				UpdateOffset();

				lastElevationOffset = ElevationOffset;
			}
		}

		/// <summary>
		/// Casts the ray against a horizontal plane centered at us and returns whether they intersected it.
		/// Returns in currentPoint the point relative to us in Universe space that intersected the plane centered at us
		/// </summary>
		/// <param name="ray">The ray to test.</param>
		/// <param name="currentPoint">The point relative to us in Universe space that intersected the plane centered at us.</param>
		/// <returns>True if the ray intersects the plane, false otherwise.</returns>
		/// <since>1.3.0</since>
		private bool RaycastRelativeToCenter(Ray ray, out Vector3 currentPoint)
		{
			var planeCenter = MapComponent.transform.localToWorldMatrix.MultiplyPoint(Vector3.zero);
			var referencePlane = new Plane(Vector3.up, planeCenter);

			float castEntry;
			if (referencePlane.Raycast(ray, out castEntry))
			{
				currentPoint = MapComponent.transform.worldToLocalMatrix.MultiplyPoint(ray.GetPoint(castEntry));

				return true;
			}
			else
			{
				currentPoint = Vector3.positiveInfinity;
			}

			return false;
		}

		/// <summary>
		/// Casts the ray against a horizontal plane centered at us and returns whether they intersected it inside the extent.
		/// Returns in currentPoint the point relative to us in Universe space that intersected the plane centered at us
		/// </summary>
		/// <param name="ray">The ray to test.</param>
		/// <param name="currentPoint">The point relative to us in Universe space that intersected the plane centered at us.</param>
		/// <returns>True if the ray intersects the plane, false otherwise.</returns>
		/// <since>1.3.0</since>
		public bool Raycast(Ray ray, out Vector3 currentPoint)
		{
			RaycastRelativeToCenter(ray, out currentPoint);

			bool insideExtent = false;
			if (Shape == MapExtentShapes.Circle)
			{
				insideExtent = Vector3.Distance(currentPoint, Vector3.zero) <= Width;
			}
			else if (Shape == MapExtentShapes.Square)
			{
				insideExtent = math.abs(currentPoint.x) < Width / 2 && math.abs(currentPoint.z) < Width / 2;
			}
			else
			{
				insideExtent = math.abs(currentPoint.x) < Width / 2 && math.abs(currentPoint.z) < Height / 2;
			}

			currentPoint = MapComponent.transform.localToWorldMatrix.MultiplyPoint(currentPoint);
			currentPoint = transform.worldToLocalMatrix.MultiplyPoint(currentPoint);

			return insideExtent;
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				return;
			}

			PreUpdateTabletop();
		}

		private void UpdateOffset()
		{
			var newPosition = TransformWrapper.localPosition;

			newPosition.y = (float)(ElevationOffset * TransformWrapper.localScale.x);

			TransformWrapper.localPosition = newPosition;
		}
	}
}

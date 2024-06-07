// COPYRIGHT 1995-2022 ESRI
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
using Esri.ArcGISMapsSDK.Utils.GeoCoord;
using Esri.ArcGISMapsSDK.Utils.Math;
using Esri.GameEngine.Geometry;
using Esri.HPFramework;
using Unity.Mathematics;
using UnityEngine;

namespace Esri.ArcGISMapsSDK.Components
{
	[DisallowMultipleComponent]
	[ExecuteAlways]
	[RequireComponent(typeof(HPTransform))]
	[AddComponentMenu("ArcGIS Maps SDK/ArcGIS Location")]
	public class ArcGISLocationComponent : MonoBehaviour
	{
		[SerializeField]
		private ArcGISPoint position = null;

		[SerializeField]
		private ArcGISRotation rotation;

		protected ArcGISMapComponent mapComponent;
		private HPTransform hpTransform;
		private bool internalHasChanged = false;
		private double3 universePosition;
		private quaternion universeRotation;

		public ArcGISPoint Position
		{
			get => position;
			set
			{
				if (position == null || !position.IsValid || !position.Equals(value))
				{
					position = value;

					internalHasChanged = true;

					SyncPositionWithHPTransform();
				}
			}
		}

		public ArcGISRotation Rotation
		{
			get => rotation;
			set
			{
				if (!value.Equals(rotation))
				{
					rotation = value;

					internalHasChanged = true;

					SyncPositionWithHPTransform();
				}
			}
		}

		protected void Awake()
		{
			if (position != null && position.IsValid)
			{
				// Ensure HPTransform is sync'd from geoPosition, rather than geoPosition being sync'd from HPTransform
				internalHasChanged = true;
			}
		}

		private void Initialize()
		{
			mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

			if (mapComponent == null)
			{
				Debug.LogError("Unable to find a parent ArcGISMapComponent.");

				enabled = false;
				return;
			}

			hpTransform = GetComponent<HPTransform>();

			mapComponent.MapTypeChanged += () =>
			{
				internalHasChanged = true;
				SyncPositionWithHPTransform();
			};

			// When SR changes recalculate from geographic position
			mapComponent.View.SpatialReferenceChanged += () =>
			{
				internalHasChanged = true;
			};
		}

		protected void LateUpdate()
		{
			if (mapComponent == null)
			{
				return;
			}

			SyncPositionWithHPTransform();
		}

		protected void OnEnable()
		{
			Initialize();
		}

		protected void OnTransformParentChanged()
		{
			Initialize();
		}

		private void PullChangesFromHPTransform()
		{
			universePosition = hpTransform.UniversePosition;
			universeRotation = hpTransform.UniverseRotation;

			var cartesianPosition = universePosition;
			var cartesianRotation = universeRotation.ToQuaterniond();

			var newPosition = mapComponent.View.WorldToGeographic(cartesianPosition); // May result in NaN position

			if (position != null && position.IsValid)
			{
				// this try catch is necessary because the below mentioned example could try to project between 2 SR's that cannot be projected between
				try
				{
					// When creating a location component with a specific SR and then sliding it around or updating the HPTransform
					// this method can change the SR of the Location component which is strange behavior
					this.position = GeoUtils.ProjectToSpatialReference(newPosition, position.SpatialReference); // this is a no-op if the sr is already the same
				}
				catch
				{
					this.position = newPosition;
				}
			}
			else
			{
				this.position = newPosition;
			}

			this.rotation = GeoUtils.FromCartesianRotation(cartesianPosition, cartesianRotation, mapComponent.View.SpatialReference, mapComponent.MapType);
		}

		private void PushChangesToHPTransform()
		{
			var cartesianPosition = mapComponent.View.GeographicToWorld(position);

			if (!cartesianPosition.IsValid())
			{
				// If the geographic position is not a valid cartesian position, ignore it
				PullChangesFromHPTransform(); // Reset position from current, assumed value, cartesian position

				return;
			}

			var cartesianRotation = GeoUtils.ToCartesianRotation(cartesianPosition, rotation, mapComponent.View.SpatialReference, mapComponent.MapType);

			universePosition = cartesianPosition;
			universeRotation = cartesianRotation.ToQuaternion();

			hpTransform.UniversePosition = universePosition;
			hpTransform.UniverseRotation = universeRotation;
		}

		public static void SetPositionAndRotation(GameObject gameObject, ArcGISPoint geographicPosition, ArcGISRotation geographicRotation)
		{
			var locationComponent = gameObject.GetComponent<ArcGISLocationComponent>();

			if (locationComponent)
			{
				locationComponent.Position = geographicPosition;
				locationComponent.Rotation = geographicRotation;

				return;
			}

			var hpTransform = gameObject.GetComponent<HPTransform>();

			if (!hpTransform)
			{
				throw new System.InvalidOperationException(gameObject.name + " requires an HPTransform");
			}

			var mapComponent = gameObject.GetComponentInParent<ArcGISMapComponent>();

			if (!mapComponent)
			{
				throw new System.InvalidOperationException(gameObject.name + " should a child of a ArcGISMapComponent");
			}

			var spatialReference = mapComponent.View.SpatialReference;

			if (spatialReference == null)
			{
				throw new System.InvalidOperationException("View must have a spatial reference");
			}

			var cartesianPosition = mapComponent.View.GeographicToWorld((ArcGISPoint)geographicPosition);

			hpTransform.UniversePosition = cartesianPosition;
			hpTransform.UniverseRotation = GeoUtils.ToCartesianRotation(cartesianPosition, geographicRotation, spatialReference, mapComponent.MapType).ToQuaternion();
		}

		public void SyncPositionWithHPTransform()
		{
			if (mapComponent == null || mapComponent.View.SpatialReference == null)
			{
				// Defer until we have a spatial reference
				return;
			}

			if (internalHasChanged && position != null && position.IsValid)
			{
				PushChangesToHPTransform();
			}
			else if (position == null || !position.IsValid || !universePosition.Equals(hpTransform.UniversePosition) || !universeRotation.Equals(hpTransform.UniverseRotation))
			{
				PullChangesFromHPTransform();
			}

			internalHasChanged = false;
		}
	}
}

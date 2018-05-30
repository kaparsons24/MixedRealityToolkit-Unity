﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Internal.Definitions.Physics;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Internal.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.Internal.Managers;
using System.Collections;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.InputSystem.Pointers
{
    /// <summary>
    /// Base Class for pointers that don't inherit from MonoBehaviour.
    /// </summary>
    public class GenericPointer : IMixedRealityPointer
    {
        public GenericPointer(string pointerName, IMixedRealityInputSource inputSourceParent)
        {
            InputSystem = MixedRealityManager.Instance.GetManager<IMixedRealityInputSystem>();
            PointerId = InputSystem.FocusProvider.GenerateNewPointerId();
            PointerName = pointerName;
            InputSourceParent = inputSourceParent;
        }

        public IMixedRealityInputSystem InputSystem { get; }

        public uint PointerId { get; }

        public string PointerName { get; set; }

        public IMixedRealityInputSource InputSourceParent { get; }

        public IMixedRealityCursor BaseCursor { get; set; }

        public ICursorModifier CursorModifier { get; set; }

        public ITeleportTarget TeleportTarget { get; set; }

        public bool InteractionEnabled { get; set; }

        public bool FocusLocked { get; set; }

        public float? PointerExtent { get; set; }

        public RayStep[] Rays { get; protected set; } = { new RayStep(Vector3.zero, Vector3.forward) };

        public LayerMask[] PrioritizedLayerMasksOverride { get; set; }

        public IMixedRealityFocusHandler FocusTarget { get; set; }

        public IPointerResult Result { get; set; }

        public IBaseRayStabilizer RayStabilizer { get; set; }

        public virtual void OnPreRaycast()
        {
            Ray pointingRay;
            if (TryGetPointingRay(out pointingRay))
            {
                Rays[0].CopyRay(pointingRay, (PointerExtent ?? InputSystem.FocusProvider.GlobalPointingExtent));
            }

            if (RayStabilizer != null)
            {
                RayStabilizer.UpdateStability(Rays[0].Origin, Rays[0].Direction);
                Rays[0].CopyRay(RayStabilizer.StableRay, (PointerExtent ?? InputSystem.FocusProvider.GlobalPointingExtent));
            }
        }

        public virtual void OnPostRaycast() { }

        public virtual bool TryGetPointerPosition(out Vector3 position)
        {
            position = Vector3.zero;
            return false;
        }

        public virtual bool TryGetPointingRay(out Ray pointingRay)
        {
            pointingRay = default(Ray);
            return false;
        }

        public virtual bool TryGetPointerRotation(out Quaternion rotation)
        {
            rotation = Quaternion.identity;
            return false;
        }

        #region IEquality Implementation

        public static bool Equals(IMixedRealityPointer left, IMixedRealityPointer right)
        {
            return left.Equals(right);
        }

        bool IEqualityComparer.Equals(object left, object right)
        {
            return left.Equals(right);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (ReferenceEquals(this, obj)) { return true; }
            if (obj.GetType() != GetType()) { return false; }

            return Equals((IMixedRealityPointer)obj);
        }

        private bool Equals(IMixedRealityPointer other)
        {
            return other != null && PointerId == other.PointerId && string.Equals(PointerName, other.PointerName);
        }

        int IEqualityComparer.GetHashCode(object obj)
        {
            return obj.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = 0;
                hashCode = (hashCode * 397) ^ (int)PointerId;
                hashCode = (hashCode * 397) ^ (PointerName != null ? PointerName.GetHashCode() : 0);
                return hashCode;
            }
        }

        #endregion IEquality Implementation
    }
}

using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace InputSystemActions {
    public sealed class TouchscreenInputActions {
        private readonly InputMap _inputMap;
        private Vector2 _primaryTouchPosition;
        private bool _isPrimaryTouchActive;

        public event Action<Vector2> SingleSwipeDelta;
        public event Action DoubleTouchActive;
        public event Action<Vector2, Vector2> DoubleTouchPositions;

        public TouchscreenInputActions(InputMap inputMap) {
            _inputMap = inputMap;

            _inputMap.GameplayInput.Enable();

            _inputMap.GameplayInput.PrimaryTouchPosition.performed += PrimaryTouchPosition;
            _inputMap.GameplayInput.SecondTouchPosition.performed += DoubleTouch;

            _inputMap.GameplayInput.PrimaryTouchContact.started += _ => PrimaryTouchOnEnable();
            _inputMap.GameplayInput.PrimaryTouchContact.canceled += _ => PrimaryTouchOnDisable();

            _inputMap.GameplayInput.DoubleTouchContact.started += _ => SecondTouchOnEnable();
            _inputMap.GameplayInput.DoubleTouchContact.canceled += _ => SecondTouchOnDisable();

            SingleSwipeOnEnable();
        }

        #region SyngleSwipe 

            private void PrimaryTouchOnEnable() {
                _isPrimaryTouchActive = true;
            }

             private void PrimaryTouchOnDisable() {
                _isPrimaryTouchActive = false;
            }

            private void PrimaryTouchPosition(InputAction.CallbackContext context) {
                _primaryTouchPosition = context.ReadValue<Vector2>();
            }

            private void SingleSwipeOnEnable() {
                _inputMap.GameplayInput.SingleSwipeOnScreen.performed += SingleSwipe;
            }

            private void SingleSwipeOnDisable() {
                _inputMap.GameplayInput.SingleSwipeOnScreen.performed -= SingleSwipe;
            }

            private void SingleSwipe(InputAction.CallbackContext context) {
                SingleSwipeDelta?.Invoke(context.ReadValue<Vector2>());
            }

        #endregion

        #region DoubleTouch

            private void SecondTouchOnEnable() {
                DoubleTouchActive?.Invoke();
                SingleSwipeOnDisable();
            }

            private void SecondTouchOnDisable() {
                SingleSwipeOnEnable();
            }

            private void DoubleTouch(InputAction.CallbackContext context) {
                if (_isPrimaryTouchActive) {
                    DoubleTouchPositions?.Invoke(_primaryTouchPosition, context.ReadValue<Vector2>());
                }
            }

        #endregion
    }
}
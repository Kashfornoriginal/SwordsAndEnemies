using Data.Player;
using UnityEngine;
using Services.Input;
using Services.PersistentProgress;
using Services.ServiceLocator;
using UnityEngine.SceneManagement;

namespace Hero
{
    public class HeroMovement : MonoBehaviour, IProgressSavable, IProgressLoadable
    {
        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed;
        private IInputService _inputService;
        private Camera _camera;

        private void Awake()
        {
            _inputService = AllServices.Container.Single<IInputService>();
        }

        private void Update()
        {
            Vector3 movementVector = Vector3.zero;

            movementVector = TryGetMovementVector(movementVector);

            movementVector += Physics.gravity;
            
            _characterController.Move(movementVector * (Time.deltaTime * _speed));
        }

        public void SetUp(Camera cam)
        {
            _camera = cam;
        }

        public void UpdateProgress(PlayerProgress playerProgress)
        {
            playerProgress.WorldData.PositionOnLevel = new PositionOnLevel(GetCurrentLevel(),
                transform.position.AsVectorData());
        }

        public void LoadProgress(PlayerProgress playerProgress)
        {
            var worldData = playerProgress.WorldData.PositionOnLevel;
            
            if (GetCurrentLevel() == worldData.LevelName)
            {
                var savedPosition = worldData.Position;
                
                if (worldData.Position != null)
                {
                    WarpPosition(savedPosition);
                }
            }
        }

        private void WarpPosition(Vector3Data savedPosition)
        {
            _characterController.enabled = false;
            transform.position = savedPosition.AsUnityVector();
            _characterController.enabled = true;
        }

        private Vector3 TryGetMovementVector(Vector3 movementVector)
        {
            if (_inputService.Axis.sqrMagnitude > Mathf.Epsilon)
            {
                movementVector = _camera.transform.TransformDirection(_inputService.Axis);
                movementVector.y = 0;
                movementVector.Normalize();

                transform.forward = movementVector;
            }

            return movementVector;
        }

        private static string GetCurrentLevel()
        {
            return SceneManager.GetActiveScene().name;
        }
    }
}

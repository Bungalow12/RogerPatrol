using System;
using UnityEngine;


namespace UnityStandardAssets._2D
{
    public class CameraFollow : MonoBehaviour
    {
        public float xMargin = 1f; // Distance in the x axis the player can move before the camera follows.
        public float yMargin = 1f; // Distance in the y axis the player can move before the camera follows.
        public float xSmooth = 8f; // How smoothly the camera catches up with it's target movement in the x axis.
        public float ySmooth = 8f; // How smoothly the camera catches up with it's target movement in the y axis.
        public Vector2 maxXAndY; // The maximum x and y coordinates the camera can have.
        public Vector2 minXAndY; // The minimum x and y coordinates the camera can have.

        protected Vector2 maxViewportXAndY; // The maximum x and y coordinates the camera can have.
        protected Vector2 minViewportXAndY; // The minimum x and y coordinates the camera can have.

        [SerializeField]
        private Transform player; // Reference to the player's transform.

        public void SetPlayer(Transform player)
        {
            this.player = player;
        }

        private void Awake()
        {
            Camera camera = Camera.main;
            float height = 2.0f * camera.orthographicSize;
            float width = height * camera.aspect;
            float halfHeight = camera.orthographicSize;
            float halfWidth = width / 2;

            maxViewportXAndY = new Vector2(maxXAndY.x - halfWidth, maxXAndY.y - halfHeight);
            minViewportXAndY = new Vector2(minXAndY.x + halfWidth, minXAndY.y + halfHeight);
        }


        private bool CheckXMargin()
        {
            // Returns true if the distance between the camera and the player in the x axis is greater than the x margin.
            return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
        }


        private bool CheckYMargin()
        {
            // Returns true if the distance between the camera and the player in the y axis is greater than the y margin.
            return Mathf.Abs(transform.position.y - player.position.y) > yMargin;
        }


        private void Update()
        {
            try
            {
                TrackPlayer();
            }
            catch(MissingReferenceException)
            {
                this.enabled = false;
            }
        }


        private void TrackPlayer()
        {
            if (this.player != null)
            {
                // By default the target x and y coordinates of the camera are it's current x and y coordinates.
                float targetX = transform.position.x;
                float targetY = transform.position.y;

                // If the player has moved beyond the x margin...
                if (CheckXMargin())
                {
                    // ... the target x coordinate should be a Lerp between the camera's current x position and the player's current x position.
                    targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth*Time.deltaTime);
                }

                // If the player has moved beyond the y margin...
                if (CheckYMargin())
                {
                    // ... the target y coordinate should be a Lerp between the camera's current y position and the player's current y position.
                    targetY = Mathf.Lerp(transform.position.y, player.position.y, ySmooth*Time.deltaTime);
                }

                // The target x and y coordinates should not be larger than the maximum or smaller than the minimum.
                targetX = Mathf.Clamp(targetX, minViewportXAndY.x, maxViewportXAndY.x);
                targetY = Mathf.Clamp(targetY, minViewportXAndY.y, maxViewportXAndY.y);

                // Set the camera's position to the target position with the same z component.
                transform.position = new Vector3(targetX, targetY, transform.position.z);

            }
        }
    }
}

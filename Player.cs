﻿using SplashKitSDK;

namespace FlappyBird
{

    public class Player
    {
        public Bitmap _PlayerBitmap;
        public double X { get; private set; }
        public double Y { get; private set; }
        public int Width { get { return _PlayerBitmap.Width; } }
        public int Height { get { return _PlayerBitmap.Height; } }

        private float _gravity = 0.5f;
        private float _jumpForce = -5.0f;
        private float _velocity = 0;

        public Player(Window gameWindow)
        {
            _PlayerBitmap = new Bitmap("Player", "images/dove.png");
            X = gameWindow.Width * (30.0 / 100);
            Y = (gameWindow.Height - Height) / 2;
        }

        // Method to set the player bitmap
        public void SetPlayerBitmap(Bitmap bitmap)
        {
            _PlayerBitmap = bitmap;
        }

        // Method to reset the player state
        public void Reset(Window gameWindow)
        {
            X = gameWindow.Width * (30.0 / 100); // Reset X to initial position
            Y = (gameWindow.Height - Height) / 2; // Reset Y to initial position
            _velocity = 0; // Reset velocity to 0
        }

        // Draws the player on the window
        public void Draw()
        {
            // Update bird position
            Y += _velocity;

            // Calculate rotation angle based on velocity
            float rotationAngle = _velocity * 2;

            // Rotate the bird's bitmap
            DrawingOptions options = SplashKit.OptionRotateBmp(rotationAngle);
            _PlayerBitmap.Draw(X, Y, options);
        }

        // Listens for relevant input keys and performs a respective action
        public void HandleInput()
        {
            // Check if the space key is pressed to initiate a jump
            if (SplashKit.KeyDown(KeyCode.SpaceKey))
            {
                // Apply upward velocity
                _velocity = _jumpForce;
            }
            else
            {
                // Apply gravity to the velocity
                _velocity += _gravity;
            }
        }

        // Ensures the bird doesn't fall off-screen
        public void MaintainGround(double groundLevel)
        {
            // Ensure the bottom edge of the player stays within the y-axis bounds
            if (Y + _PlayerBitmap.Height > groundLevel)
            {
                Y = groundLevel - _PlayerBitmap.Height;
                _velocity = 0;
            }
        }

        // Checks and returns if the player is within the bounds of a game object (if the player is attempting to pass through a gap)
        public bool IsWithinGameObjectBounds(GameObject gameObject)
        {
            return X + Width > gameObject.X && X < gameObject.X + gameObject._ObjectBitmap.Width;
        }

        // Checks and returns if the player collides/intersects with a game object
        public bool CollidedWith(GameObject gameObject)
        {
            return _PlayerBitmap.BitmapCollision(X, Y, gameObject._ObjectBitmap, gameObject.X, gameObject.Y);
        }

        // Checks and returns if the player passes through a gap successfully.
        // This is a dependent check that relies on the IsWithinGameObjectBounds check above
        // Therefore, this should follow the IsWithinGameObjectBounds for the same gameObject
        public bool PassesGap(GameObject gameObject)
        {
            return X + GameObject.SPEED >= gameObject.X + gameObject._ObjectBitmap.Width;
        }
    }

    // Delegate that defines the method signature for the collision methods we defined above
    delegate bool Collision(GameObject gameObject);
}

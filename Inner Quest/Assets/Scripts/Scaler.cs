using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InnerQuest
{
    public class Scaler
    {
        // Reference and current resolution
        Vector2 _refResolution;
        Vector2 _currResolution;

        // Value to change between pixels and Unity Units
        float _unityUds;

        /// <summary>
        /// Constructor of the class. Receives the reference resolution and the
        /// actual resolution as parameters. Also receives the size of the camera
        /// and calculates how many pixels will translate to one Unity unit. 
        /// </summary>
        /// <param name="res">Actual resolution of the screen</param>
        /// <param name="refRes">Reference resolution for scaling</param>
        /// <param name="camSize">Size of the camera</param>
        public Scaler(Vector2 res, Vector2 refRes, int camSize)
        {
            // Assign resolutions to intern variables for storage
            _currResolution = res;
            _refResolution = refRes;

            // Calculate how many pixels per unity unit
            _unityUds = res.y / (2 * camSize);
        }

        /// <summary>
        /// Function that scales a sprite or rectangle to fit the screen
        /// </summary>
        /// <param name="sizeInUnits">How many Unity Units occupies a sprite</param>
        /// <param name="scale">Scale of the object in scene</param>
        /// <returns>Returns the new scale calculated</returns>
        public Vector3 ScaleToFitScreen(Vector3 sizeInUnits, Vector3 scale)
        {
            // Temporal variable for some of the calculations
            Vector3 temp = sizeInUnits;

            // Convert units to pixels
            temp.x *= _unityUds;
            temp.y *= _unityUds;

            // Set the width of the 
            temp.x = _currResolution.x;

            // Scale the height proportionally
            temp.y = (temp.x * sizeInUnits.y) / sizeInUnits.x;

            // Convert to unity units
            temp.x = temp.x / _unityUds;
            temp.y = temp.y / _unityUds;

            // New scale to apply on the object
            Vector3 nScale;

            // Calculate the scale using the function
            nScale = resizeObjectScale(sizeInUnits, temp, scale);

            return nScale;
        }

        /// <summary>
        /// Scales a rectangle using another as reference mainting it's aspect ratio. 
        /// </summary>
        /// <param name="srcDims">The rectangle that is going to be scaled</param>
        /// <param name="refDims">Reference rectangle to be used</param>
        /// <returns>New dimensions of the rectangle</returns>
        public Vector2 ScaleToFitKeepingAspectRatio(Vector2 srcDims, Vector2 refDims)
        {
            Vector2 temp = srcDims;

            // Check width and scale object keeping aspect ratio
            if (temp.x > refDims.x || temp.x < refDims.x)
            {
                // Set width to fit the new rectangle
                temp.x = refDims.x;

                // Scale height proportionally
                temp.y = (temp.x * srcDims.y) / srcDims.x;
            }

            // Then check height
            if (temp.y > refDims.y)
            {
                // If the height keeps being higher, 
                // initialize again the dimensions
                if (temp != srcDims)
                {
                    temp = srcDims;
                }

                // Set height to fit the reference one
                temp.y = refDims.y;

                // Scale width proportionally 
                temp.x = (temp.y * srcDims.x) / srcDims.y;
            }

            return temp;
        }

        /// <summary>
        /// Calculates the new scale to apply on an object. 
        /// </summary>
        /// <param name="origUnits">Units that occupies an object originally</param>
        /// <param name="currUnits">The units that will occupy the object after scalling</param>
        /// <param name="scale">Actual scale the object has</param>
        /// <returns>New scale calculated</returns>
        public Vector3 resizeObjectScale(Vector3 origUnits, Vector3 currUnits, Vector3 scale)
        {
            // New scale that will be applied
            Vector3 scalated = new Vector3();

            // Calculate the scale
            scalated.x = (currUnits.x * scale.x) / origUnits.x;
            scalated.y = (currUnits.y * scale.y) / origUnits.y;

            return scalated;
        }

        /// <summary>
        /// Calculate the new scale keeping aspect ratio of the object.
        /// </summary>
        /// <param name="origUnits">Units that occupies an object originally</param>
        /// <param name="currUnits">The units that will occupy the object after scalling</param>
        /// <param name="scale">Actual scale the object has</param>
        /// <returns>New scale calculated</returns>
        public Vector3 resizeObjectScaleKeepingAspectRatio(Vector3 origUnits, Vector3 currUnits, Vector3 scale)
        {
            // New scale to apply on the object
            Vector3 scalated = new Vector3();

            // Check width of the object
            if (origUnits.x > currUnits.x)
            {
                // Calculate new scale
                scalated.x = scalated.y = (currUnits.x * scale.x) / origUnits.x;
            }

            // Check height of the object
            if (origUnits.y > currUnits.y)
            {
                // If new scale has been calculated
                if (scalated.x != 0 && scalated.y != 0)
                {
                    // Reboot scale
                    scalated.x = scalated.y = 0;
                }

                // Calculate new scale
                scalated.y = scalated.x = (currUnits.y * scale.y) / origUnits.y;
            }

            return scalated;
        }

        /// <summary>
        /// Find the quadrant of an object to make each determined operation
        /// </summary>
        /// <param name="position">The world position (x, y) of the object that will compare</param>
        /// <returns></returns>
        public Vector2 ScreenToWorldPosition(Vector2 position)
        {
            Vector2 temp = position;

            //If the x position of the object is higher than the middle of the current resolution
            if (temp.x > (_currResolution.x / 2))
            {
                //The x position is positive and it is added to the middle in Unity units
                temp.x = (0 + (temp.x - (_currResolution.x / 2))) / _unityUds;
            }
            else
            {
                //If x position is lower then is negative in the operation and will subtract to the middle in Unity units
                temp.x = (0 - ((_currResolution.x / 2) - temp.x)) / _unityUds;
            }

            //If the y position of the object is higher than the middle of the current resolution
            if (temp.y > (_currResolution.y / 2))
            {
                //The y position is positive and it is added to the middle in Unity units
                temp.y = (0 + (temp.y - (_currResolution.y / 2))) / _unityUds;
            }
            else
            {
                //If y position is lower then it is negative in the operation and will subtract to the middle in Unity units
                temp.y = (0 - ((_currResolution.y / 2) - temp.y)) / _unityUds;
            }

            return temp;
        }

        /// <summary>
        /// Resize a number with the width of the screen and the reference resolution width
        /// </summary>
        /// <param name="x">Value to be scaled</param>
        /// <returns>The value scaled width the different resolutions</returns>
        public float ResizeX(float x)
        {
            return (x * _currResolution.x) / _refResolution.x;
        }

        /// <summary>
        /// Resize the height with the screen resolution and reference resolution
        /// </summary>
        /// <param name="y">Height to be scaled</param>
        /// <returns>The new value</returns>
        public float ResizeY(float y)
        {
            return (y * _currResolution.y) / _refResolution.y;
        }

        /// <summary>
        /// Get the value to convert pixels to Unity Units and 
        /// Unity Units to pixels.
        /// </summary>
        /// <returns>The value calculated in the constructor</returns>
        public float UnityUds()
        {
            return _unityUds;
        }

        /// <summary>
        /// Get the value of the actual game resolution (width x height)
        /// to use it in other operations
        /// </summary>
        /// <returns>The value of the actual resolution</returns>
        public Vector2 CurrentResolution()
        {
            return _currResolution;
        }
    } // Scaler
} // namespace

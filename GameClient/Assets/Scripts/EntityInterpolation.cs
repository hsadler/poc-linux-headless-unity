using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityInterpolation : MonoBehaviour
{

    private Vector3 velocity = Vector3.zero;

    public void InterpolateToPosition(Vector3 newPosition)
    {
        if(this.transform.position != newPosition)
        {
            var d = Vector3.Distance(this.transform.position, newPosition);
            if (d < 1f)
            {

                // pure linear interpolation from point to point
                //this.transform.position = Vector3.Lerp(
                //    this.transform.position,
                //    newPosition,
                //    1f
                //);

                // alt interpolation method (adds smooth curve)
                this.transform.position = Vector3.SmoothDamp(
                    this.transform.position,
                    newPosition,
                    ref this.velocity,
                    0.01F
                );

                // alt interpolation method (seems choppier)
                //this.transform.position = Vector3.MoveTowards(
                //    this.transform.position,
                //    newPosition,
                //    100 * Time.deltaTime
                //);

            }
            else
            {
                this.transform.position = newPosition;
            }
        }
    }

}
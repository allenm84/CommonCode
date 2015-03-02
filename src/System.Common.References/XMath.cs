using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace System.Common.References
{
  public static class XMath
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="scopePosition"></param>
    /// <param name="scopeSize"></param>
    /// <param name="targetPosition"></param>
    /// <param name="targetSize"></param>
    /// <param name="projectileSpeed"></param>
    /// <returns></returns>
    public static Vector2 DetermineOrientation(Vector2 scopePosition, Vector2 scopeSize, 
      Vector2 targetPosition, Vector2 targetSize, Vector2 targetVelocity, 
      float projectileSpeed)
    {
      Vector2 position = (targetPosition + targetSize * .5f) - (scopePosition + scopeSize * .5f);
      Vector2 velocity = targetVelocity;

      float a = projectileSpeed * projectileSpeed - velocity.LengthSquared();
      float b = Vector2.Dot(position, velocity);
      float c = position.LengthSquared();
      // Cope with rare special case where bullet and target have same speed, to avoid dividing by zero  
      if (a == 0)
      {
        if (b < 0)
        {
          // Meet halfway...  
          float time = -0.5f * c / b;
          return (position + velocity * time) / (projectileSpeed * time);
        }
        else
        {
          // Can't hit target
        }
      }
      else
      {
        float bSqPlusAC = b * b + a * c;
        // Can't take square root of negative number  
        if (bSqPlusAC >= 0)
        {
          float solution = (b + (float)Math.Sqrt(bSqPlusAC)) / a;
          if (solution >= 0)
          {
            float time = solution;
            return (position + velocity * time) / (projectileSpeed * time);
          }
          else
          {
            // Can't hit target  
          }
        }
        else
        {
          // Can't hit target  
        }
      }

      return Vector2.Zero;
    }
  }
}

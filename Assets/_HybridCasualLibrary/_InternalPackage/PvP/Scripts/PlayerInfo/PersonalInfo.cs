using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class PersonalInfo
{
    #region Properties
    /// <summary>
    /// Getter Properties
    /// </summary>
    public abstract bool isLocal { get; }
    public abstract string userId { get; }
    public abstract string name { get; }
    public abstract Sprite avatar { get; }
    public abstract Sprite avatarFrame { get; }
    public abstract string countryCode { get; }
    public abstract Sprite nationalFlag { get; }
    #endregion

    #region Methods
    /*
     * Query data
     */
    public abstract float GetTotalNumOfPoints(); // Represent rank points of that player
    /*
     * Insert & Update data
     */
    public abstract void SetTotalNumOfPoints(float amount); // Represent rank points of that player
    #endregion
}
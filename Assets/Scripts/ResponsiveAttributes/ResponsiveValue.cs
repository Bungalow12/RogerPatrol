using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// The type of modification to perform on Responsive values.
/// </summary>
public enum ModificationStyle
{
    ADDITIVE,
    MULTIPLICATIVE,
    MULTIPLICATIVE_FLOOR,
    MULTIPLICATIVE_CEILING
}

/// <summary>
/// Integer value that adjusts based on modification style and delta value.
/// </summary>
[Serializable]
public struct ResponsiveInt
{
    [SerializeField]
    int baseValue;

    [SerializeField]
    float deltaValue;

    [SerializeField]
    ModificationStyle typeOfModification;

    /// <summary>
    /// Instantiates a new Responsive Integer.
    /// </summary>
    /// <param name="newBaseValue">The starting base value.</param>
    /// <param name="newDeltaValue">The delta to adjust by.</param>
    /// <param name="newModificationType">The type of mathematical adjustment.</param>
    public ResponsiveInt(int newBaseValue, float newDeltaValue, ModificationStyle newModificationType = ModificationStyle.ADDITIVE)
    {
        baseValue = newBaseValue;
        deltaValue = newDeltaValue;
        typeOfModification = newModificationType;
    }

    /// <summary>
    /// Gets the delta at the current difficulty level.
    /// </summary>
    /// <returns>The current delta.</returns>
    private float CurrentDelta
    {
        get
        {
            return deltaValue * Globals.Level;
        }
    }

    /// <summary>
    /// The adjusted value based on delta and level.
    /// </summary>
    /// <returns>The adjusted value.</returns>
    public int Value
    {
        get
        {
            if (typeOfModification == ModificationStyle.ADDITIVE)
            {
                return baseValue + (int)CurrentDelta;
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE)
            {
                return baseValue * (int)CurrentDelta;
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE_FLOOR)
            {
                return baseValue * Mathf.FloorToInt(CurrentDelta);
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE_CEILING)
            {
                return baseValue * Mathf.CeilToInt(CurrentDelta);
            }

            return baseValue;
        }
        set
        {
            this.baseValue = value;
        }
    }
}

/// <summary>
/// Floating point value that adjusts based on modification style and delta value.
/// </summary>
[Serializable]
public struct ResponsiveFloat
{
    [SerializeField]
    float baseValue;

    [SerializeField]
    float deltaValue;

    [SerializeField]
    ModificationStyle typeOfModification;

    /// <summary>
    /// Instantiates a new Responsive Float.
    /// </summary>
    /// <param name="newBaseValue">The starting base value.</param>
    /// <param name="newDeltaValue">The delta to adjust by.</param>
    /// <param name="newModificationType">The type of mathematical adjustment.</param>
    public ResponsiveFloat( float newBaseValue, float newDeltaValue, ModificationStyle newModificationType = ModificationStyle.ADDITIVE )
    {
        baseValue = newBaseValue;
        deltaValue = newDeltaValue;
        typeOfModification = newModificationType;
    }

    /// <summary>
    /// Gets the delta at the current difficulty level.
    /// </summary>
    /// <returns>The current delta.</returns>
    private float CurrentDelta
    {
        get
        {
            return deltaValue * Globals.Level;
        }
    }

    /// <summary>
    /// The adjusted value based on delta and level.
    /// </summary>
    /// <returns>The adjusted value.</returns>
    public float Value
    {
        get
        {
            if (typeOfModification == ModificationStyle.ADDITIVE)
            {
                return baseValue + CurrentDelta;
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE)
            {
                return baseValue * CurrentDelta;
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE_FLOOR)
            {
                return baseValue * Mathf.Floor(CurrentDelta);
            }

            if (typeOfModification == ModificationStyle.MULTIPLICATIVE_CEILING)
            {
                return baseValue * Mathf.Ceil(CurrentDelta);
            }

            return baseValue;
        }
        set
        {
            this.baseValue = value;
        }
    }
}


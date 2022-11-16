using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager : MonoBehaviour
{
    #region Animations_Tags
    public const string CHARACTER_RUN_ANIMATION_TAG = "isCharacterMoving";
    public const string CHARACTER_CROUCH_ANIMATION_TAG = "isCharacterCrouching";
    public const string CHARACTER_CLIMB_WALL_ANIMATION_TAG = "isCharacterClimbingWall";
    public const string CHARACTER_JUMP_ANIMATION_TAG = "characterJump";
    public const string CHARACTER_SLIDE_ANIMATION_TAG = "characterSlide"; //Confirmar
    public const string CHARACTER_DASH_ANIMATION_TAG = "characterDash";
    public const string CHARACTER_SHOOT_ANIMATION_TAG = "characterShoot";
    public const string CHARACTER_DIE_ANIMATION_TAG = "characterDeath";
    #endregion
}

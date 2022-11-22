using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : MonoBehaviour
{
    Animator characterAnimator;

    private void Awake()
    {
        characterAnimator = GetComponent<Animator>();
    }

    public void RunAnimation(bool _isCharacterRunning)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_RUN_ANIMATION_TAG, _isCharacterRunning);
    }
    public void CrouchAnimation(bool _isCrouching)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_CROUCH_ANIMATION_TAG, _isCrouching);
    }
    public void ClimbWallAnimation(bool _isClimbing)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_CLIMB_WALL_ANIMATION_TAG, _isClimbing);
    }
    public void SlideAnimation(bool _isSliding)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_SLIDE_ANIMATION_TAG, _isSliding);
    }
    public void DashAnimation(bool _isDashing)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_DASH_ANIMATION_TAG, _isDashing);
    }
    public void ShootAnimation(bool _isShooting)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_SHOOT_ANIMATION_TAG, _isShooting);
    }
    public void DeathAnimation(bool _isDying)
    {
        characterAnimator.SetBool(TagManager.CHARACTER_DIE_ANIMATION_TAG, _isDying);
    }
}

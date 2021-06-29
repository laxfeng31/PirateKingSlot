using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class script_fish_ai : MonoBehaviour
{
    public Transform targetTransform;
    public Transform this_transform;
    public script_spawn_fish script_sf;
    public float min_tween_duration = 7f;
    public float max_tween_duration = 12f;
    private void Awake()
    {
        DOTween.Init();
        this_transform = transform;
        script_sf = GameObject.Find("obj_spawn_point").GetComponent<script_spawn_fish>();
    }
    public void setTargetArea(Transform target, bool isJellyFish) {
        targetTransform = target;
        StartCoroutine(MoveToTarget(isJellyFish));
    }
    public IEnumerator MoveToTarget(bool isJellyFish)
    {
        float tweenTimer = Random.Range(min_tween_duration, max_tween_duration);
        if (targetTransform.transform.position.x - this_transform.position.x < 0)
        {
            if (isJellyFish)
            {
                this_transform.up = (targetTransform.transform.position - this_transform.position).normalized;
            }
            else
            {
                this_transform.right = -(targetTransform.transform.position - this_transform.position).normalized;
            }
            if (targetTransform) moveTween(this_transform, targetTransform, tweenTimer, Ease.Linear,true);
        }
        else
        {
            if (isJellyFish)
            {
                this_transform.up = -(targetTransform.transform.position - this_transform.position).normalized;
                this_transform.localScale = new Vector2(this_transform.transform.localScale.x, this_transform.transform.localScale.y * -1);
            } else
            {
                this_transform.right = -(targetTransform.transform.position - this_transform.position).normalized;
                this_transform.localScale = new Vector2(this_transform.transform.localScale.x, this_transform.transform.localScale.y * -1);
            }
            if (targetTransform) moveTween(this_transform,targetTransform,tweenTimer,Ease.Linear, true);
        }
        
        yield return new WaitForSeconds(tweenTimer);
        script_sf.destroyFirstItemInList();
        Destroy(targetTransform.gameObject);
        Destroy(gameObject);
    }

    public void moveTween(Transform self, Transform target, float duration,Ease default_ease,bool isRandom)
    {
        if (target)
        {

            if (isRandom)
            {
                int int_ease = Random.Range(0, 8);
                switch (int_ease)
                {
                    
                    case 7:
                        self.DOMove(target.position, duration).SetEase(Ease.InSine);
                        break;
                    case 6:
                        self.DOMove(target.position, duration).SetEase(Ease.OutSine);
                        break;
                    case 5:
                        self.DOMove(target.position, duration).SetEase(Ease.InOutSine);
                        break;
                    case 4:
                        self.DOMove(target.position, duration).SetEase(Ease.InQuad);
                        break;
                    case 3:
                        self.DOMove(target.position, duration).SetEase(Ease.OutQuad);
                        break;
                    case 2:
                        self.DOMove(target.position, duration).SetEase(Ease.InOutQuad);
                        break;
                    case 1:
                        self.DOMove(target.position, duration).SetEase(Ease.Linear);
                        break;
                    case 0:
                        self.DOMove(target.position, duration).SetEase(Ease.InOutCubic);
                        break;
                    default:
                        self.DOMove(target.position, duration).SetEase(default_ease);
                        break;
                }
            }
            else
            {
                self.DOMove(target.position, duration).SetEase(default_ease);
            }

        }
    }
}

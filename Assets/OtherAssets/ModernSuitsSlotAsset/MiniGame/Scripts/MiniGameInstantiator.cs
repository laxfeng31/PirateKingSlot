using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mkey;
using UnityEngine.UI;

namespace MkeyFW
{
    public class MiniGameInstantiator : MonoBehaviour
    {
        public GameObject fortuneWheelPrefab;
        public int FortuneWheelResult { get; private set; }
        public EaseAnim ease = EaseAnim.EaseOutBack;
        public GameObject MiniGame { get; private set; }
    

        public void Create(bool autoStart)
        {
            if (fortuneWheelPrefab == null) return;
            if (fortuneWheelPrefab)
            {
                if (MiniGame) Destroy(MiniGame);
                MiniGame = Instantiate(fortuneWheelPrefab);
                MiniGame.transform.position = transform.position;
                MiniGame.transform.parent = transform;
                MiniGame.transform.localScale = Vector3.zero;
                SimpleTween.Value(gameObject, 0, 1, 0.25f)
                    .SetOnUpdate((float val) => { MiniGame.transform.localScale = new Vector3(val, val, val); })
                    .SetEase(ease);

                SimpleTween.Value(gameObject, 0, 1, 0.5f).AddCompleteCallBack(() =>
                {
                    if (autoStart) MiniGame.GetComponent<WheelController>().StartSpin((res, isbigwin) => 
                    {
                        SlotPlayer.Instance.AddCoins(res);
                        Destroy(MiniGame, 3);
                    });
                });
            }
        }
    }
}
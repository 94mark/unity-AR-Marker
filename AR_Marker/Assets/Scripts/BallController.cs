using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallController : MonoBehaviour
{
    public float resetTime = 3.0f;
    public float captureRate = 0.3f;  //포획 확률(30%)
    public Text result;
    public GameObject effect;

    Rigidbody rb;
    bool isReady = true;
    Vector2 startPos;
    // Start is called before the first frame update
    void Start()
    {
        //포획 결과 텍스트를 공백 상태로 초기화한다
        result.text = "";

        //리지드바디의 물리 능력을 비활성화
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(!isReady)
        {
            return;
        }

        //공을 카메라 전방 하단에 배치한다
        SetBallPosition(Camera.main.transform);

        if(Input.touchCount > 0 && isReady)
        {
            Touch touch = Input.GetTouch(0);

            //만일 터치를 시작했다면
            if(touch.phase == TouchPhase.Began)
            {
                //터치를 시작한 픽셀을 저장한다
                startPos = touch.position;
            }
            //터치가 끝났다면
            else if(touch.phase == TouchPhase.Ended)
            {
                //손가락이 드래그한 픽셀의 y축 거리를 구한다
                float dragDistance = touch.position.y - startPos.y;

                //AR 카메라를 기준으로 던질 방향(전방 45도 위쪽)을 설정한다
                Vector3 throwAngle = (Camera.main.transform.forward + Camera.main.transform.up).normalized;

                //물리 능력을 활성화하고 준비 상태를 false로 바꿔 놓는다
                rb.isKinematic = false;
                isReady = false;

                //던질 방향 * 손가락 드래그 거리만큼 공에 물리적 힘을 가한다
                rb.AddForce(throwAngle * dragDistance * 0.005f, ForceMode.VelocityChange);

                //3초 후에 공의 위치 및 속도를 초기화
                Invoke("ResetBall", resetTime);
            }
        }
    }

    void SetBallPosition(Transform anchor)
    {
        //카메라의 위치에서 일정 거리만큼 떨어진 특정 위치를 설정
        Vector3 offset = anchor.forward * 0.5f + anchor.up * -0.2f;
        //공의 위치를 카메라 위치에서 특정 위치만큼 이동된 거리로 정함
        transform.position = anchor.position + offset;
    }


    private void ResetBall()
    {
        //물리능력을 비활성화하고 속도도 초기화한다
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;

        //준비 상태로 변경한다
        isReady = true;

        gameObject.SetActive(true);
    }


    private void OnCollisionEnter(Collision collision)
    {
        //준비 상태라면 충돌 처리 이벤트 함수를 그냥 종료시킨다
        if(isReady)
        {
            return;
        }
        //포획 확률을 추첨한다(0 ~ 1.0 사이의 실수)
        float draw = Random.Range(0, 1.0f);

        if(draw <= captureRate)
        {
            result.text = "포획 성공!";
        }
        else
        {
            result.text = "포획에 실패해 도망쳤습니다...";
        }
        //이펙트를 생성한다
        Instantiate(effect, collision.transform.position, Camera.main.transform.rotation);

        //고양이 캐릭터를 제거하고 공을 비활성화한다
        Destroy(collision.gameObject);
        gameObject.SetActive(false);
    }
}

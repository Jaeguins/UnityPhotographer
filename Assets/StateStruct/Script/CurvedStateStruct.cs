using UnityEngine;
using System.Threading;
namespace Custom {
    public partial class CurvedStateStruct<T> {

        /// <summary>
        /// 생성 메소드
        /// </summary>
        /// <param name="initial">초기값</param>
        /// <param name="adder">덧셈 연산</param>
        /// <param name="multiplier">실수와의 곱셈 연산</param>
        /// <param name="time">변하는데 걸리는 시간(초)(마지막 To()로부터)</param>
        /// <returns>객체</returns>
        public static CurvedStateStruct<T> Init(T initial, DAdd<T> adder, DMultiply<T> multiplier, float time = 1.0f) {
            CurvedStateStruct<T> ret = new CurvedStateStruct<T> {
                add = adder,
                multiply = multiplier,
                Start = initial,
                Target = initial,
                Now = initial,
                Timer = time
            };
            ret.Running = false;
            ret.SetCurve(AnimationCurve.Linear(0, 0, 1, 1));
            runningQueue.Add(ret);
            if (!runner.IsAlive)
                runner=new Thread(new ThreadStart(ThreadRun));
                runner.Start();
            return ret;
        }

        /// <summary>
        /// 안전하게 제거하기 위하여 연결을 끊는 메소드, 해제 직전에 호출하세요
        /// </summary>
        public void RemoveSafely() {
            runningQueue.Remove(this);
        }
    }
    public partial class CurvedStateStruct<T> {

        public AnimationCurve Curve { get; private set; }

        /// <summary>
        /// 변화 시작값
        /// </summary>
        public T Start { get; protected set; }

        /// <summary>
        /// 변화 도착값
        /// </summary>
        public T Target { get; protected set; }

        /// <summary>
        /// 현재 값
        /// </summary>
        public T Now { get; protected set; }

        /// <summary>
        /// 변하는데 걸리는 시간(초)
        /// </summary>
        public float Timer { get; protected set; }

        /// <summary>
        /// 변화 진행도(0~1)
        /// </summary>
        public float Factor { get; protected set; }

        /// <summary>
        /// 실행 중인지에 대한 불값
        /// </summary>
        public bool Running { get; protected set; }

        /// <summary>
        /// 해당 타입의 현재값으로 자동캐스팅
        /// </summary>
        /// <param name="v">호출되는 객체</param>
        public static implicit operator T(CurvedStateStruct<T> v) {
            return v.Now;
        }

        /// <summary>
        /// 시간 업데이트 메소드.
        /// 실행중 변경하면 그시점의 상태로부터 그시점의 목표로 해당 시간만큼 애니메이션 됩니다.
        /// </summary>
        /// <param name="val"></param>
        public void SetTimer(float val) {
            Timer = val;
            if (Running)
                To(Target);
        }

        /// <summary>
        /// 커브 업데이트 메소드.
        /// 실행중 변경하면 그시점의 상태로부터 그시점의 목표로 해당 시간만큼 애니메이션 됩니다.
        /// </summary>
        /// <param name="val"></param>
        public void SetCurve(AnimationCurve val) {
            Curve = val;
            if (Running)
                To(Target);
        }

        /// <summary>
        /// 덧셈 연산 델리게이트
        /// </summary>
        protected DAdd<T> add;

        /// <summary>
        /// 곱셈 연산 델리게이트
        /// </summary>
        protected DMultiply<T> multiply;

        /// <summary>
        /// 목표 상태 변경.
        /// 시간은 초기화되어 시간에 맞춰 움직입니다.
        /// </summary>
        /// <param name="target">목표값</param>
        public void To(T target) {
            Target = target;
            Start = Now;
            Factor = 1;
            Running = true;
        }
    }
    public partial class CurvedStateStruct<T> : IndividualRunner {

        /// <summary>
        /// 한번의 순차마다 변하는것에 대한 메소드.
        /// </summary>
        /// <param name="deltaTime">경과시간</param>
        protected override void Run(float deltaTime) {
            if (Running) {
                if (Factor < 0) {
                    Running = false;
                    Now = Target;
                    return;
                }
                Now = add(multiply(Curve.Evaluate(Factor), Start), multiply(Curve.Evaluate(1 - Factor), Target));
                Factor -= (deltaTime / Timer);
            }
        }
    }
}

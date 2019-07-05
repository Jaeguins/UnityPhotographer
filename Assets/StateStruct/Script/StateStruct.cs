using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Custom {

    /// <summary>
    /// 덧셈 연산에 대한 델리게이트, left+right에 대한 정의를 해주시면 됩니다.
    /// </summary>
    /// <typeparam name="T">대상 클래스</typeparam>
    /// <param name="left">더할 객체 1</param>
    /// <param name="right">더할 객체 2</param>
    /// <returns>결과값</returns>
    public delegate T DAdd<T>(T left, T right);

    /// <summary>
    /// 실수와의 곱셈 연산에 대한 델리게이트, left*right에 대한 정의를 해주시면 됩니다.
    /// </summary>
    /// <typeparam name="T">대상 클래스</typeparam>
    /// <param name="left">배율로 곱할 실수</param>
    /// <param name="right">대상 객체</param>
    /// <returns>결과값</returns>
    public delegate T DMultiply<T>(float left, T right);

    /// <summary>
    /// 분리된 스레드에서 동작 가능한지에 대한 추상클래스
    /// </summary>
    public abstract class IndividualRunner {

        /// <summary>
        /// 스레드에서 돌릴 인스턴스들
        /// </summary>
        protected static List<IndividualRunner> runningQueue = new List<IndividualRunner>();

        /// <summary>
        /// 시간이 지남에 따라 실행할 메서드
        /// </summary>
        /// <param name="deltaTime"></param>
        protected abstract void Run(float deltaTime);

        /// <summary>
        /// 시간측정기
        /// </summary>
        private static Stopwatch stopwatch = new Stopwatch();

        /// <summary>
        /// 스레드
        /// </summary>
        protected static Thread runner;

        /// <summary>
        /// 스레드가 실행중인지에 대한 값
        /// </summary>
        public static bool IsAlive {
            get {
                return runner.IsAlive;
            }
        }

        /// <summary>
        /// 실행 중인 인스턴스 수
        /// </summary>
        public static int Count {
            get {
                return runningQueue.Count;
            }
        }

        /// <summary>
        /// 스레드 실행 메서드
        /// </summary>
        protected static void ThreadRun() {
            float deltaTime;
            while (runningQueue.Count > 0) {
                stopwatch.Stop();
                deltaTime = (float)stopwatch.Elapsed.TotalMilliseconds;
                for (int i = 0; i < runningQueue.Count; i++) {
                    if (runningQueue[i] == null)
                        runningQueue.RemoveAt(i);
                    else
                        runningQueue[i].Run(deltaTime / 1000f);
                }
                stopwatch.Reset();
                stopwatch.Start();
                Thread.Sleep(0);
            }
        }
    }

    /// <summary>
    /// T에 대해 상태를 저장하고, 갱신하는 컨테이너 (생성자 사용금지)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class StateStruct<T> {

        /// <summary>
        /// 생성 메소드
        /// </summary>
        /// <param name="initial">초기값</param>
        /// <param name="adder">덧셈 연산</param>
        /// <param name="multiplier">실수와의 곱셈 연산</param>
        /// <param name="time">변하는데 걸리는 시간(초)(마지막 To()로부터)</param>
        /// <returns>객체</returns>
        public static StateStruct<T> Init(T initial, DAdd<T> adder, DMultiply<T> multiplier, float time = 1.0f) {
            StateStruct<T> ret = new StateStruct<T> {
                add = adder,
                multiply = multiplier,
                Start = initial,
                Target = initial,
                Now = initial,
                Timer = time
            };
            ret.Running = false;
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
    
    public partial class StateStruct<T> {

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
        public static implicit operator T(StateStruct<T> v) {
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
    
    public partial class StateStruct<T> : IndividualRunner {

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
                Now = add(multiply(Factor, Start), multiply((1 - Factor), Target));
                Factor -= (deltaTime / Timer);
            }
        }
    }
}
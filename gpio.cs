using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Device
{
    /// <summary>
    /// GPIOクラス
    /// </summary>
    class GPIO : IDisposable
    {
        public string Name { get; private set; }
        public int IoNumber { get; private set; }

        /// <summary>
        /// 仮想ディレクトリのパス
        /// </summary>
        private string DirPath;

        public GPIO(int ioNum, Direction d = Direction.In, State s = State.Lo, bool forceCreate = false, string name = "")
        {
            Name = name;            //IO名を格納
            IoNumber = ioNum;       //IO番号を格納
            string portName = "GPIO" + ioNum.ToString();
            DirPath = "/sys/class/gpio/gpio" + ioNum.ToString();

            //gpioがオープン済みでない場合のみ
            if (Directory.Exists(DirPath))
            {
                //強制作成スイッチONの時ポートの後始末
                if(forceCreate)
                    File.WriteAllText("/sys/class/gpio/unexport", ioNum.ToString());
            }
            
            File.WriteAllText("/sys/class/gpio/export", ioNum.ToString());      //exportに番号を書き込んで仮想ディレクトリを作成する


                //ポートが使用可能になったかを確認する
                if(!Directory.Exists(DirPath))
                    throw new System.InvalidOperationException("I/O Port cannot Open");

                SetDirection(d);    //信号の方向を設定

                if (d == Direction.Out)     //出力設定の時だけ端子状態を設定する
                    SetPinState(s);
        }

        /// <summary>
        /// 指定したGPIOピンの信号方向を取得します。
        /// </summary>
        /// <param name="direction"></param>
        public Direction GetDirection()
        {
            string dir = File.ReadAllText(DirPath + "/direction", Encoding.Default);

            if (dir.Contains("in"))
                return Direction.In;
            else if (dir.Contains("out"))
                return Direction.Out;
            else
                throw new System.Exception("Undefined state of direction: " + dir);
        }

        /// <summary>
        /// 入出力方向を設定
        /// </summary>
        /// <param name="d">方向</param>
        public void SetDirection(Direction d)
        {
            switch(d)
            {
                case Direction.In:
                    File.WriteAllText(DirPath + "/direction", "in");
                    break;
                case Direction.Out:
                    File.WriteAllText(DirPath + "/direction", "out");
                    break;
            }
        }

        /// <summary>
        /// 端子状態を取得
        /// </summary>
        /// <returns></returns>
        public State GetState()
        {
            string s = File.ReadAllText(DirPath + "/value", Encoding.Default);

            if (s.Contains("1"))
                return State.Hi;
            else if (s.Contains("0"))
                return State.Lo;
            else
                throw new Exception("undifned pin state: " + s);
        }

        /// <summary>
        /// I/Oの状態を設定
        /// </summary>
        /// <param name="s">端子状態</param>
        public void SetPinState(State s)
        {
            switch(s)
            {
                case State.Hi:
                    File.WriteAllText(DirPath + "/value", "1");
                    break;
                case State.Lo:
                    File.WriteAllText(DirPath + "/value", "0");
                    break;
            }
        }

        /// <summary>
        /// 端子状態を切り替える
        /// </summary>
        public void ToggleState()
        {
            if(GetDirection() == Direction.Out)
            {
                if (GetState() == State.Hi)
                    SetPinState(State.Lo);
                else
                    SetPinState(State.Hi);
            }
        }

        /// <summary>
        /// インスタンスの破棄
        /// </summary>
        public void Dispose()
        {
            File.WriteAllText("/sys/class/gpio/unexport", IoNumber.ToString());
        }

        /// <summary>
        /// 信号方向
        /// </summary>
        public enum Direction
        {
            In, Out
        }

        /// <summary>
        /// 端子状態
        /// </summary>
        public enum State
        {
            Hi,Lo
        }
    }


}

using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace Device
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //2番ピンが開いていない場合はOpenします。
            GPIO testPin = new GPIO(12, GPIO.Direction.Out, GPIO.State.Lo, true,"test");

            //Lチカタスクを作成します。
            var task = Task.Run(async () =>
            {
                //とりあえず10回ON/OFF
                foreach(var i in Enumerable.Range(1,50))
                {
                    testPin.ToggleState();      //端子状態を反転する
                    await Task.Delay(200);       //指定時間のwait
                }
            });

            //タスクを実行し、完了を待機します。
            task.Wait();
        }
    }
}

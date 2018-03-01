﻿/*************************************************
** auth： zsh2401@163.com
** date:  2018/2/26 18:41:10 (UTC +8:00)
** desc： ...
*************************************************/
using AutumnBox.Basic.Device;
using AutumnBox.Basic.Flows;
using AutumnBox.OpenFramework.Extension;
using AutumnBox.OpenFramework.Open.V1;
using System.IO;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;

namespace AutumnBox.TestMod
{
    public class Xiaomi6RecoveryFlasher : AutumnBoxExtension
    {
        public override string Name => "一键刷入小米6REC";
        public override string Auth => "zsh2401";
        public override string Description => "这个模块有可能报毒,请放行!";
        public override MailAddress ContactMail => new MailAddress("zsh2401@163.com");
        public override DeviceState RequiredDeviceState => DeviceState.Poweron | DeviceState.Recovery | DeviceState.Fastboot;
        private RecoveryFlasher flasher;
        public override void OnStartCommand(StartArgs args)
        {
            //如果设备不处于fastboot状态,将设备重启到Fastboot状态
            if (args.Device.State != DeviceState.Fastboot)
            {
                DeviceRebooter.Reboot(args.Device, RebootOptions.Fastboot);
            }
            //将rec文件从dll中提取出来
            CopyFileToLocal();
            //稍等一会儿,防止手机还未重启完毕
            Thread.Sleep(1000);
            //使用REC刷入功能流程
            flasher = new RecoveryFlasher();
            //初始化参数
            flasher.Init(new RecoveryFlasherArgs()
            {
                DevBasicInfo = args.Device,
                RecoveryFilePath = "tmp.img"
            });
            //当这个属性被设置为true时,AutumnBox窗口必将接收到功能模块完成的信息
            //否则只要Finished事件有任何的注册,就不会触发AnyFinished事件,
            //那么AutumnBox主程序就不会对这个功能流程的结束有任何处理
            //flasher.MustTiggerAnyFinishedEvent = true;
            //绑定事件
            //flasher.Finished += (s, e) =>
            //{
            //    File.Delete("tmp.img");
            //};
            //同步运行
            flasher.Run();
        }
        public override bool OnStopCommand()
        {
            flasher?.ForceStop();
            return base.OnStopCommand();
        }
        public override void OnFinished()
        {
            base.OnFinished();
            File.Delete("tmp.img");
        }
        private void CopyFileToLocal()
        {
            var type = this.GetType();
            var resourcePath = type.Namespace + ".twrp-3.2.1-0-sagit.img";
            using (var resourceStream = type.Assembly.GetManifestResourceStream(resourcePath))
            {
                using (FileStream localWriter = new FileStream("tmp.img", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    byte[] buffer = new byte[1024];
                    int len;
                    long count = 0;
                    while (true)
                    {
                        len = resourceStream.Read(buffer, 0, buffer.Length);
                        localWriter.Write(buffer, 0, len);
                        count += len;
                        if (len == 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }
}
using System;

namespace OnFlyScheduler.Abstract
{
    public delegate void CallBackMethod(TimeSpan timeOut);

    public delegate void OnExceptionCallBack(Exception exception);
    public delegate void OnStartCallBack(Job currentJob);
    public delegate void OnEndCallBack(Job currentJob);
}

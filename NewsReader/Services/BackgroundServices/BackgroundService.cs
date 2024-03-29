﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace NewsReader.Services.BackgroundServices
{
    public static class BackgroundService
    {
        public const string taskName = "BlogFeedBackgroundTask";
        public const string taskEntryPoint = "BackgroundTasks.BlogFeedBackgroundTask";



        //注册后台任务方法封装
        public static async Task<BackgroundTaskRegistration> RegisterBackgroundTask(string taskEntryPoint,
                                                                string taskName,
                                                                IBackgroundTrigger trigger,
                                                                IBackgroundCondition condition)
        {
            var status = await BackgroundExecutionManager.RequestAccessAsync();
            if (status == BackgroundAccessStatus.Unspecified || status == BackgroundAccessStatus.DeniedByUser)
            {
                return null;
            }
            foreach (var cur in BackgroundTaskRegistration.AllTasks)
            {
                if (cur.Value.Name == taskName)
                {
                    cur.Value.Unregister(true);
                }
            }
            var builder = new BackgroundTaskBuilder
            {
                Name = taskName,
                TaskEntryPoint = taskEntryPoint,
            };
            builder.SetTrigger(trigger);
            if (condition != null)
            {
                builder.AddCondition(condition);
            }
            BackgroundTaskRegistration task = builder.Register();
            return task;
        }
    }
}

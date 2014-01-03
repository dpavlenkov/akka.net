﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pigeon.Actor
{
    public class LocalActorRef : ActorRef
    {
        private ActorBase _actor;
        public ActorContext Context { get;private set; }
        public LocalActorRef(ActorPath path,ActorContext context)
        {
            this.Path = path;
            this.Context = context;
        }

        public void SetActor(ActorBase actor)
        {
            this._actor = actor;
        }

        protected override void TellInternal(object message, ActorRef sender)
        {
            this.Context.Post(sender, this, message);
        }
    }
}
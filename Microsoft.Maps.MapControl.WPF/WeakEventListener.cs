using System;

namespace Microsoft.Maps.MapControl.WPF
{
    internal class WeakEventListener<TInstance, TSource, TEventArgs> where TInstance : class
    {
        private readonly WeakReference _weakInstance;

        public Action<TInstance, TSource, TEventArgs> OnEventAction { get; set; }

        public Action<WeakEventListener<TInstance, TSource, TEventArgs>> OnDetachAction { get; set; }

        public WeakEventListener(TInstance instance)
        {
            if (instance is null)
                throw new ArgumentNullException(nameof(instance));
            _weakInstance = new WeakReference(instance);
        }

        public void OnEvent(TSource source, TEventArgs eventArgs)
        {
            var target = (TInstance)_weakInstance.Target;
            if (target is object)
            {
                if (OnEventAction is null)
                    return;
                OnEventAction(target, source, eventArgs);
            }
            else
                Detach();
        }

        public void Detach()
        {
            if (OnDetachAction is null)
                return;
            OnDetachAction(this);
            OnDetachAction = null;
        }
    }
}

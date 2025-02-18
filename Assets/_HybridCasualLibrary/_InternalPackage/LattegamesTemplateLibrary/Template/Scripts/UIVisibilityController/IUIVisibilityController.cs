using System;
using System.Collections;

namespace LatteGames
{
    public interface IUIVisibilityController
    {
        void Show();
        void Hide();
        void ShowImmediately();
        void HideImmediately();

        SubscriptionEvent GetOnStartShowEvent();
        SubscriptionEvent GetOnEndShowEvent();
        SubscriptionEvent GetOnStartHideEvent();
        SubscriptionEvent GetOnEndHideEvent();
    }

    public class SubscriptionEvent
    {
        private Action sEvent;

        public void Subscribe(Action listener)
        {
            this.sEvent += listener;
        }

        public void Unsubscribe(Action listener)
        {
            this.sEvent -= listener;
        }

        public void Invoke()
        {
            this.sEvent?.Invoke();
        }
    }
}
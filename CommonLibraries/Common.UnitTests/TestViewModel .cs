namespace Common.UnitTests
{
    using System;
    using System.Linq.Expressions;

    using Common.ViewModel;
    
    internal class TestViewModel : NotifyPropertyChangedWithLinkedPropertiesBase
    {
        public void InitLink()
        {
            //Test Chaining
            AddLinkedProperty(() => Property1, () => Property2);
            AddLinkedProperty(() => Property2, () => Property3);

            //Test multiple child
            AddLinkedProperty(() => Property4, new Expression<Func<object>>[] { () => Property5, () => Property6 });

            //Test no cycle
            AddLinkedProperty(() => Property7, () => Property8);
            AddLinkedProperty(() => Property8, () => Property7);
        }
        public void InitLinkDuplicate()
        {
            AddLinkedProperty(() => Property1, () => Property1);
        }
        public void InitLinkUnknownSource()
        {
            //Unknown because only instance | public property are allowed
            AddLinkedProperty(() => Inner, () => Property1);
        }
        public void InitLinkUnknownDestination()
        {
            //Unknown because only instance | public property are allowed
            AddLinkedProperty(() => Property1, () => Inner);
        }
        
        public static string Inner { get; set; }

        private string _property1;
        private string _property2;
        private string _property3;
        private string _property4;
        private string _property5;
        private string _property6;
        private string _property7;
        private string _property8;

        public string Property8
        {
            get { return _property8; }
            set
            {
                if (value != _property8)
                {
                    _property8 = value;
                    OnNotifyPropertyChanged(() => Property8);
                }
            }
        }
        public string Property7
        {
            get { return _property7; }
            set
            {
                if (value != _property7)
                {
                    _property7 = value;
                    OnNotifyPropertyChanged(() => Property7);
                }
            }
        }
        public string Property6
        {
            get { return _property6; }
            set
            {
                if (value != _property6)
                {
                    _property6 = value;
                    OnNotifyPropertyChanged(() => Property6);
                }
            }
        }
        public string Property5
        {
            get { return _property5; }
            set
            {
                if (value != _property5)
                {
                    _property5 = value;
                    OnNotifyPropertyChanged(() => Property5);
                }
            }
        }
        public string Property4
        {
            get { return _property4; }
            set
            {
                if (value != _property4)
                {
                    _property4 = value;
                    OnNotifyPropertyChanged(() => Property4);
                }
            }
        }
        public string Property3
        {
            get { return _property3; }
            set
            {
                if (value != _property3)
                {
                    _property3 = value;
                    OnNotifyPropertyChanged(() => Property3);
                }
            }
        }
        public string Property2
        {
            get { return _property2; }
            set
            {
                if (value != _property2)
                {
                    _property2 = value;
                    OnNotifyPropertyChanged(() => Property2);
                }
            }
        }
        public string Property1
        {
            get { return _property1; }
            set
            {
                if (value != _property1)
                {
                    _property1 = value;
                    OnNotifyPropertyChanged(() => Property1);
                }
            }
        }
    }
}
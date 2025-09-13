namespace Common.ViewModel.UnitTests
{
    using System;

    using Common.ViewModel.Command;
    using Common.ViewModel.Menu;

    using NUnit.Framework;

    [TestFixture]
    public class MenuViewModelTest
    {
        [Test]
        public void TestConstructor()
        {
            MenuViewModel vm = new MenuViewModel();

            Assert.That(vm.MenuText, Is.Null);
            Assert.That(vm.IsSeparator, Is.False);
            Assert.That(vm.Command, Is.Null);
            Assert.That(vm.CommandParameter, Is.Null);
            Assert.That(vm.IsChecked, Is.False);
            Assert.That(vm.IsCheckable, Is.False);
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
        [Test]
        public void TestConstructor2()
        {
            MenuViewModel vm = new MenuViewModel("Text");

            Assert.That(vm.MenuText, Is.EqualTo("Text"));
            Assert.That(vm.IsSeparator, Is.False);
            Assert.That(vm.Command, Is.Null);
            Assert.That(vm.CommandParameter, Is.Null);
            Assert.That(vm.IsChecked, Is.False);
            Assert.That(vm.IsCheckable, Is.False);
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
        [Test]
        public void TestConstructor3()
        {
            RelayCommand cmd = new RelayCommand(o => { });
            MenuViewModel vm = new MenuViewModel("Text", cmd);

            Assert.That(vm.MenuText, Is.EqualTo("Text"));
            Assert.That(vm.IsSeparator, Is.False);
            Assert.That(vm.Command, Is.EqualTo(cmd));
            Assert.That(vm.CommandParameter, Is.Null);
            Assert.That(vm.IsChecked, Is.False);
            Assert.That(vm.IsCheckable, Is.False);
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
        [Test]
        public void TestConstructor4()
        {
            RelayCommand cmd = new RelayCommand(o => { });
            object parameter = new object();
            MenuViewModel vm = new MenuViewModel("Text", cmd, parameter);

            Assert.That(vm.MenuText, Is.EqualTo("Text"));
            Assert.That(vm.IsSeparator, Is.False);
            Assert.That(vm.Command, Is.EqualTo(cmd));
            Assert.That(vm.CommandParameter, Is.EqualTo(parameter));
            Assert.That(vm.IsChecked, Is.False);
            Assert.That(vm.IsCheckable, Is.False);
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
        [Test]
        public void TestSeparator()
        {
            MenuViewModel vm = MenuViewModel.Separator();

            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.MenuText, Is.Null);
            Assert.That(vm.IsSeparator, Is.True);
            Assert.That(vm.Command, Is.Null);
            Assert.That(vm.CommandParameter, Is.Null);
            Assert.That(vm.IsChecked, Is.False);
            Assert.That(vm.IsCheckable, Is.False);
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
        [Test]
        public void TestProperty()
        {
            MenuViewModel vm = new MenuViewModel();

            Assert.That(vm.IsCheckable, Is.False);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.IsCheckable = true;
                Assert.That(vm.IsCheckable, Is.True);
                checker.Check(nameof(vm.IsCheckable));
            }

            Assert.That(vm.IsChecked, Is.False);
            using (NotifyPropertyChangedChecker checker = new NotifyPropertyChangedChecker(vm))
            {
                vm.IsChecked = true;
                Assert.That(vm.IsChecked, Is.True);
                checker.Check(nameof(vm.IsChecked));
            }
        }
        [Test]
        public void TestChild()
        {
            MenuViewModel vm = new MenuViewModel();
            MenuViewModel child1 = new MenuViewModel();
            MenuViewModel child2 = new MenuViewModel();
            MenuViewModel child3 = new MenuViewModel();

            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));

            vm.AddChild(child1);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child1 }));

            vm.AddChild(child2);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child1, child2 }));

            // Try readd existing
            vm.AddChild(child1);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child1, child2 }));

            vm.RemoveChild(child1);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child2 }));

            // Remove not present child
            vm.RemoveChild(child3);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child2 }));

            vm.AddChild(child3);
            Assert.That(vm.HasChild, Is.True);
            Assert.That(vm.Children, Is.EquivalentTo(new[] { child2, child3 }));

            vm.RemoveAllChildren();
            Assert.That(vm.HasChild, Is.False);
            Assert.That(vm.Children, Is.EquivalentTo(Array.Empty<MenuViewModel>()));
        }
    }
}
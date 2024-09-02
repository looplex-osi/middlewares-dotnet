using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.Entities.Abstractions;

public class ObservableTypeTests
{
    [TestClass]
    public class SampleModelTests
    {
        [TestMethod]
        public void PropertyChange_ShouldBeTracked()
        {
            // Arrange
            var model = new SampleModel();

            // Act
            model.Name = "Test Name";
            model.Age = 30;

            // Assert
            Assert.IsTrue(model.ChangedProperties.Contains("Name"));
            Assert.IsTrue(model.ChangedProperties.Contains("Age"));
            Assert.AreEqual(2, model.ChangedProperties.Count);
        }

        [TestMethod]
        public void CollectionChange_ShouldBeTracked()
        {
            // Arrange
            var model = new SampleModel();
            var item1 = "Item 1";
            var item2 = "Item 2";

            // Act
            model.Items.Add(item1);
            model.Items.Add(item2);
            model.Items.Remove(item1);

            // Assert
            Assert.IsTrue(model.AddedItems.ContainsKey("Items"));
            Assert.IsTrue(model.AddedItems["Items"].Contains(item1));
            Assert.IsTrue(model.AddedItems["Items"].Contains(item2));
            Assert.AreEqual(2, model.AddedItems["Items"].Count);

            Assert.IsTrue(model.RemovedItems.ContainsKey("Items"));
            Assert.IsTrue(model.RemovedItems["Items"].Contains(item1));
            Assert.AreEqual(1, model.RemovedItems["Items"].Count);
        }

        [TestMethod]
        public void MultiplePropertyChanges_ShouldTrackAllChanges()
        {
            // Arrange
            var model = new SampleModel();

            // Act
            model.Name = "First Name";
            model.Age = 25;
            model.Name = "Second Name"; // Change Name again

            // Assert
            Assert.AreEqual(2, model.ChangedProperties.Count);
            Assert.IsTrue(model.ChangedProperties.Contains("Name"));
            Assert.IsTrue(model.ChangedProperties.Contains("Age"));
        }

        [TestMethod]
        public void CollectionChanged_ShouldTrackAddAndRemove()
        {
            // Arrange
            var model = new SampleModel();

            // Act
            model.Items.Add("Item 1");
            model.Items.Remove("Item 1");

            // Assert
            Assert.IsTrue(model.AddedItems.ContainsKey("Items"));
            Assert.IsTrue(model.AddedItems["Items"].Contains("Item 1"));
            Assert.AreEqual(1, model.AddedItems["Items"].Count);

            Assert.IsTrue(model.RemovedItems.ContainsKey("Items"));
            Assert.IsTrue(model.RemovedItems["Items"].Contains("Item 1"));
            Assert.AreEqual(1, model.RemovedItems["Items"].Count);
        }
    }
}

class SampleModel : ObservableType
{
    private string _name;
    private int _age;
    private ObservableCollection<string> _items;

    public SampleModel()
    {
        Items = new ObservableCollection<string>();
    }

    public override IList<string> ChangedProperties { get; } = new List<string>();
    public override IDictionary<string, IList<object>> AddedItems { get; } = new Dictionary<string, IList<object>>();
    public override IDictionary<string, IList<object>> RemovedItems { get; } = new Dictionary<string, IList<object>>();

    public string Name
    {
        get => _name;
        set
        {
            if (value != _name)
            {
                _name = value;
                OnPropertyChanged();
            }
        }
    }

    public int Age
    {
        get => _age;
        set
        {
            if (value != _age)
            {
                _age = value;
                OnPropertyChanged();
            }
        }
    }

    public ObservableCollection<string> Items
    {
        get => _items;
        set
        {
            if (_items != value)
            {
                _items = value;
                if (value is INotifyCollectionChanged collection)
                    BindOnCollectionChanged(ref collection);
            }
        }
    }
}
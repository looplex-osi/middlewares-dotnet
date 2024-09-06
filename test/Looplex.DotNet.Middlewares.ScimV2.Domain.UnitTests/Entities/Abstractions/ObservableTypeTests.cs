using System.Collections.ObjectModel;
using FluentAssertions;
using Looplex.DotNet.Middlewares.ScimV2.Domain.Entities.Abstractions;

namespace Looplex.DotNet.Middlewares.ScimV2.Domain.UnitTests.Entities.Abstractions;

[TestClass]
public class ObservableTypeTests
{
    [TestMethod]
    public void PropertyChange_ShouldBeTracked()
    {
        // Arrange
        var model = SampleModel.Mock();

        // Act
        model.Name = "Test Name";
        model.Age = 30;

        // Assert
        Assert.IsTrue(model.ChangedProperties.Contains("Name"));
        Assert.IsTrue(model.ChangedProperties.Contains("Age"));
        Assert.AreEqual(2, model.ChangedProperties.Count);
        
        model.AddedItems.Should().BeEmpty();
        model.RemovedItems.Should().BeEmpty();
    }

    [TestMethod]
    public void CollectionChange_ShouldBeTracked()
    {
        // Arrange
        var model = SampleModel.Mock();
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
        
        model.ChangedProperties.Should().BeEmpty();
    }

    [TestMethod]
    public void MultiplePropertyChanges_ShouldTrackAllChanges()
    {
        // Arrange
        var model = SampleModel.Mock();

        // Act
        model.Name = "First Name";
        model.Age = 25;
        model.Name = "Second Name"; // Change Name again

        // Assert
        Assert.AreEqual(2, model.ChangedProperties.Count);
        Assert.IsTrue(model.ChangedProperties.Contains("Name"));
        Assert.IsTrue(model.ChangedProperties.Contains("Age"));
        
        model.AddedItems.Should().BeEmpty();
        model.RemovedItems.Should().BeEmpty();
    }

    [TestMethod]
    public void CollectionChanged_ShouldTrackAddAndRemove()
    {
        // Arrange
        var model = SampleModel.Mock();

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
        
        model.ChangedProperties.Should().BeEmpty();
    }
    
    [TestMethod]
    public void NewCollectionChanged_ShouldTrackAddAndRemove()
    {
        // Arrange
        var model = SampleModel.Mock();

        // Act
        model.Items = new ObservableCollection<string>();
        model.Items.Add("Item 1");
        model.Items.Remove("Item 1");

        // Assert
        Assert.IsTrue(model.AddedItems.ContainsKey("Items"));
        Assert.IsTrue(model.AddedItems["Items"].Contains("Item 1"));
        Assert.AreEqual(1, model.AddedItems["Items"].Count);

        Assert.IsTrue(model.RemovedItems.ContainsKey("Items"));
        Assert.IsTrue(model.RemovedItems["Items"].Contains("Item 1"));
        Assert.AreEqual(1, model.RemovedItems["Items"].Count);
        
        model.ChangedProperties.Should().BeEmpty();
    }
}

public class SampleModel : ObservableType
{
    public static SampleModel Mock()
    {
        return new SampleModel()
        {
            Age = 1,
            Name = "Name Init"
        }.WithObservableProxy();
    }
    public virtual string? Name { get; set; }

    public virtual int Age { get; set; }

    public virtual IList<string> Items { get; set; } = new ObservableCollection<string>();
}
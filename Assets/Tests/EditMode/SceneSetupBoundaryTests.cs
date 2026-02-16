#if UNITY_EDITOR
using NUnit.Framework;
using UnityEngine;

[TestFixture]
public class SceneSetupBoundaryTests
{
    [TearDown]
    public void TearDown()
    {
        // Clean up any GameObjects created during tests
        var allObjects = Object.FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj != null)
            {
                Object.DestroyImmediate(obj);
            }
        }
    }

    [Test]
    public void CreateBoundaries_WhenCalled_CreatesBoundariesParentWith4Children()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var boundaries = GameObject.Find("Boundaries");
        Assert.IsNotNull(boundaries, "Boundaries parent GameObject should exist");
        Assert.AreEqual(4, boundaries.transform.childCount, "Should have 4 boundary walls");
    }

    [Test]
    public void CreateBoundaries_WhenCalled_CreatesWallsWithCorrectNames()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        Assert.IsNotNull(GameObject.Find("BoundaryNorth"), "North wall should exist");
        Assert.IsNotNull(GameObject.Find("BoundarySouth"), "South wall should exist");
        Assert.IsNotNull(GameObject.Find("BoundaryEast"), "East wall should exist");
        Assert.IsNotNull(GameObject.Find("BoundaryWest"), "West wall should exist");
    }

    [Test]
    public void CreateBoundaries_WhenCalled_WallsHaveNonTriggerBoxColliders()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var names = new[] { "BoundaryNorth", "BoundarySouth", "BoundaryEast", "BoundaryWest" };
        foreach (var name in names)
        {
            var wall = GameObject.Find(name);
            Assert.IsNotNull(wall, $"{name} should exist");
            var collider = wall.GetComponent<BoxCollider>();
            Assert.IsNotNull(collider, $"{name} should have a BoxCollider");
            Assert.IsFalse(collider.isTrigger, $"{name} BoxCollider should not be a trigger");
        }
    }

    [Test]
    public void CreateBoundaries_WhenCalled_WallsAreStatic()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var names = new[] { "BoundaryNorth", "BoundarySouth", "BoundaryEast", "BoundaryWest" };
        foreach (var name in names)
        {
            var wall = GameObject.Find(name);
            Assert.IsNotNull(wall, $"{name} should exist");
            Assert.IsTrue(wall.isStatic, $"{name} should be static");
        }
    }

    [Test]
    public void CreateBoundaries_WhenCalled_WallsHaveNoRendererOrMeshFilter()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var names = new[] { "BoundaryNorth", "BoundarySouth", "BoundaryEast", "BoundaryWest" };
        foreach (var name in names)
        {
            var wall = GameObject.Find(name);
            Assert.IsNotNull(wall, $"{name} should exist");
            var renderer = wall.GetComponent<MeshRenderer>();
            Assert.IsNull(renderer, $"{name} should not have a MeshRenderer (invisible)");
            var meshFilter = wall.GetComponent<MeshFilter>();
            Assert.IsNull(meshFilter, $"{name} should not have a MeshFilter (invisible)");
        }
    }

    [Test]
    public void CreateBoundaries_WhenCalled_NorthWallPositionedCorrectly()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var wall = GameObject.Find("BoundaryNorth");
        Assert.IsNotNull(wall);
        Assert.AreEqual(new Vector3(0f, 1f, 10.5f), wall.transform.position,
            "North wall should be at (0, 1, 10.5)");
        Assert.AreEqual(new Vector3(20f, 2f, 1f), wall.transform.localScale,
            "North wall should be scaled (20, 2, 1)");
    }

    [Test]
    public void CreateBoundaries_WhenCalled_SouthWallPositionedCorrectly()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var wall = GameObject.Find("BoundarySouth");
        Assert.IsNotNull(wall);
        Assert.AreEqual(new Vector3(0f, 1f, -10.5f), wall.transform.position,
            "South wall should be at (0, 1, -10.5)");
        Assert.AreEqual(new Vector3(20f, 2f, 1f), wall.transform.localScale,
            "South wall should be scaled (20, 2, 1)");
    }

    [Test]
    public void CreateBoundaries_WhenCalled_EastWallPositionedCorrectly()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var wall = GameObject.Find("BoundaryEast");
        Assert.IsNotNull(wall);
        Assert.AreEqual(new Vector3(10.5f, 1f, 0f), wall.transform.position,
            "East wall should be at (10.5, 1, 0)");
        Assert.AreEqual(new Vector3(1f, 2f, 20f), wall.transform.localScale,
            "East wall should be scaled (1, 2, 20)");
    }

    [Test]
    public void CreateBoundaries_WhenCalled_WestWallPositionedCorrectly()
    {
        var setup = new SceneSetup();
        setup.CreateBoundaries();

        var wall = GameObject.Find("BoundaryWest");
        Assert.IsNotNull(wall);
        Assert.AreEqual(new Vector3(-10.5f, 1f, 0f), wall.transform.position,
            "West wall should be at (-10.5, 1, 0)");
        Assert.AreEqual(new Vector3(1f, 2f, 20f), wall.transform.localScale,
            "West wall should be scaled (1, 2, 20)");
    }
}
#endif

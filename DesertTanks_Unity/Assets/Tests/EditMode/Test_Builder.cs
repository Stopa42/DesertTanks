using System.Collections.Generic;
using NUnit.Framework;
using RTSTutorial;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Tests.EditMode
{
    public class Test_Builder
    {
        public class Test_CanPlaceBuilding
        {
            [Test]
            public void Test_Returns_True_When_InRange()
            {
                //Arrange
                var gameObject = new GameObject();
                var building = gameObject.AddComponent<Building>();
                gameObject.transform.position = Vector3.zero;
                var layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
                
                var buildings = new List<Building>();
                buildings.Add(building);
                    
                var builder = new Builder(buildings, 5f*5f, layerMask);
                var bounds = new Bounds();
                
                //Act
                var canPlace = builder.CanPlaceBuilding(Vector3.forward * 4f, bounds, Quaternion.identity);
                
                //Assert
                Assert.That(canPlace, Is.True);
            }
            
            [Test]
            public void Test_Returns_False_When_OutOfRange()
            {
                //Arrange
                var gameObject = new GameObject();
                var building = gameObject.AddComponent<Building>();
                gameObject.transform.position = Vector3.zero;
                var layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

                var buildings = new List<Building>();
                buildings.Add(building);
                    
                var builder = new Builder(buildings, 3f*3f, layerMask);
                var bounds = new Bounds();
                
                //Act
                var canPlace = builder.CanPlaceBuilding(Vector3.forward * 4f, bounds, Quaternion.identity);
                
                //Assert
                Assert.That(canPlace, Is.False);
            }
            
            [Test]
            public void Test_Returns_False_When_Blocked()
            {
                //Arrange
                var gameObject = new GameObject();
                var building = gameObject.AddComponent<Building>();
                var collider = gameObject.AddComponent<BoxCollider>();
                collider.center = Vector3.zero;
                collider.size = Vector3.one;
                gameObject.transform.position = Vector3.zero;
                var layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));
                
                var buildings = new List<Building>();
                buildings.Add(building);
                    
                var builder = new Builder(buildings, 3f*3f, layerMask);
                var bounds = new Bounds(Vector3.zero, Vector3.one);
                
                //Act
                var canPlace = builder.CanPlaceBuilding(Vector3.zero, bounds, Quaternion.identity);
                
                //Assert
                Assert.That(canPlace, Is.False);
            }
        }
    }
}
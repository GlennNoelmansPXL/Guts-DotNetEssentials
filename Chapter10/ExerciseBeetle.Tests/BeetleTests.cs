﻿using Guts.Client.Classic;
using Guts.Client.Shared;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace BeetleGame.Tests
{
    [ExerciseTestFixture("dotNet1", "H10", "BeetleGame", @"BeetleGame\Beetle.cs"),
     Apartment(ApartmentState.STA)]
    public class BeetleTests
    {
        private object _beetleObject = null;
        
        private const string _beetleTypeName = "BeetleGame.Beetle";
        private const string _beetleAssembly = "BeetleGame";
        private readonly Type _beetleType = Type.GetType($"{_beetleTypeName}, {_beetleAssembly}");
        private int _beetleSize;
        private int _beetleX;
        private int _beetleY;
        private double _beetleSpeed;

        private Canvas _testCanvas;
       
        [SetUp]
        public void Setup()
        {
            _testCanvas = new Canvas
            {
                Width = 536, // mimic the same size as in the starter file
                Height = 356
            };
            _beetleSize = 10;
            _beetleX = 40;
            _beetleY = 35;
            _beetleSpeed = 0.0;
            _beetleObject = CreateBeetle(_testCanvas, _beetleX, _beetleY, _beetleSize);
        }

        [TearDown]
        public void TearDown()
        { }

        [MonitoredTest("Beetle - There should be a class named Beetle"), Order(1)]
        public void _1_ShouldHaveAClassNamedBeetle()
        {
            Assert.That(_beetleType, Is.Not.Null, $"There should be a class named {_beetleTypeName}");
        }

        [MonitoredTest("Beetle - Beetle class should have a parameterized constructor"), Order(2)]
        public void _2_ShouldHaveParameterizedConstructor()
        {
            var constructor = GetConstructor();
            Assert.That(constructor, Is.Not.Null,
                    () => $"{_beetleTypeName} should have a constructor with parameters (Canvas canvas, int x, int y, int size)");
        }

        [MonitoredTest("Beetle - Beetle class should have all required properties"), Order(3)]
        public void _3_ShouldHaveAllProperties()
        {
            var properties = _beetleObject.GetType().GetProperties();
            string[] expectedPropertyNames = { "Speed", "X", "Y", "Size", "Right", "Up", "IsVisible" };
            Type[] expectedPropertyTypes = {typeof(double), typeof(int), typeof(int), typeof(int),
                                            typeof(bool), typeof(bool), typeof(bool)};
            for (int i = 0; i < expectedPropertyNames.Length; i++)
            {
                AssertProperty(properties, expectedPropertyNames[i], expectedPropertyTypes[i],
                           $"{_beetleTypeName} should have a property named ${expectedPropertyNames[i]} of type ${expectedPropertyTypes[i]}.");
            }

        }

        [MonitoredTest("Beetle - Should create a valid Beetle when invoking constructor"), Order(4)]
        public void _4_ShouldCreateValidBeetleWhenInvokingContructor()
        {
            Assert.That(_beetleObject, Is.Not.Null, $"Could not create an instance of class {_beetleTypeName}");
            AssertPropertyValue(_beetleObject, "X", _beetleX);
            AssertPropertyValue(_beetleObject, "Y", _beetleY);
            AssertPropertyValue(_beetleObject, "Size", _beetleSize);
            AssertPropertyValue(_beetleObject, "Up", true);
            AssertPropertyValue(_beetleObject, "Right", true);
            AssertPropertyValue(_beetleObject, "Speed", _beetleSpeed);
        }

        [MonitoredTest("Beetle - Should create a Beetle object with ellipse on its canvas"), Order(5)]
        public void _5_ShouldCreateABeetleWithAnEllipseOnItsCanvas()
        {
            Assert.That(_beetleObject, Is.Not.Null, $"Could not create an instance of class {_beetleTypeName}");
            Assert.That(_testCanvas.Children.Count, Is.GreaterThan(0), $"Beetle should have a Canvas member with an ellipse");
            Assert.That(_testCanvas.Children[0], Is.TypeOf(typeof(Ellipse)), $"Beetle should have a Canvas member with an ellipse");

            // Check correct size and location of ellipse
            var beetleEllipse = (Ellipse)_testCanvas.Children[0];
            Assert.That(beetleEllipse.Width, Is.EqualTo(_beetleSize), $"Ellipse on canvas should have Width ({_beetleSize})");
            Assert.That(beetleEllipse.Height, Is.EqualTo(_beetleSize), $"Ellipse on canvas should have Height ({_beetleSize})");
            var beetleMargin = beetleEllipse.Margin;
            Assert.That(beetleMargin.Left, Is.EqualTo(_beetleX - (_beetleSize / 2)), $"X-Coordinate of ellipse on canvas should be {_beetleX - (_beetleSize / 2)}");
            Assert.That(beetleMargin.Top, Is.EqualTo(_beetleY - (_beetleSize / 2)), $"Y-Coordinate of ellipse on canvas should be {_beetleY - (_beetleSize / 2)}");
        }

        [MonitoredTest("Beetle - Should move up and right without hitting border")]
        public void _6_ShouldMoveUpAndRightWithoutHittingBorder()
        {
            InvokeChangePosition(_beetleObject);
            // verify the beetle went in up and right direction
            AssertPropertyValue(_beetleObject, "X", _beetleX + 1);
            AssertPropertyValue(_beetleObject, "Y", _beetleY - 1);
        }

        private void InvokeChangePosition(object beetleObject)
        {
            var methodName = "ChangePosition";
            var type = _beetleObject.GetType();
            var method = type.GetRuntimeMethod(methodName, new Type[] { });
            Assert.That(method, Is.Not.Null, $"Should have method {methodName} without arguments and void return type");
            method.Invoke(beetleObject, new object[] { });
        }

        private object CreateBeetle(Canvas canvas, int x, int y, int size)
        {
            object[] parameters = new object[] { _testCanvas, _beetleX, _beetleY, _beetleSize };
            return Activator.CreateInstance(_beetleType, parameters);
        }

        private void AssertProperty(PropertyInfo[] properties, string expectedPropertyName,
                                    Type expectedPropertyType, string message)
        {
            var property = properties.FirstOrDefault(p => p.Name == expectedPropertyName
                                                     && p.PropertyType == expectedPropertyType);
            Assert.That(property, Is.Not.Null, () => message);
        }

        private void AssertPropertyValue(object obj, string propertyName, object expectedValue)
        {
            var property = obj.GetType().GetProperty(propertyName);
            Assert.That(property.GetValue(obj), Is.EqualTo(expectedValue), 
                $"Beetle property {propertyName} has value ({property.GetValue(obj)}) but expected ({expectedValue})");
        }

        private ConstructorInfo GetConstructor()
        {
            return _beetleObject.GetType().GetConstructor(new Type[]
                                {
                                    typeof(Canvas),
                                    typeof(int),
                                    typeof(int),
                                    typeof(int)
                                });
        }
    }
}

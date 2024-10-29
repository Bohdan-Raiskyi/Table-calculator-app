using NUnit.Framework;
using System;
using System.Linq;
using Microsoft.Maui.Controls;
using tables;

namespace tables.Tests
{
    [TestFixture]
    public class MainPageTests
    {
        private MainPage _mainPage;

        [SetUp]
        public void Setup()
        {
            _mainPage = new MainPage();
        }

        [Test]
        public void Addition()
        { 
            double result = _mainPage.EvaluateExpression("5 + 3");
            Assert.AreEqual(8, result);
        }

        [Test]
        public void Subtraction()
        { 
            double result = _mainPage.EvaluateExpression("10 - 4");
            Assert.AreEqual(6, result);
        }

        [Test]
        public void Multiplication()
        { 
            double result = _mainPage.EvaluateExpression("7 * 6");
            Assert.AreEqual(42, result);
        }

        [Test]
        public void Division()
        { 
            double result = _mainPage.EvaluateExpression("8 / 2");
            Assert.AreEqual(4, result);
        }

        [Test]
        public void GreaterThan1()
        { 
            double result = _mainPage.EvaluateExpression("10 > 5");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void GreaterThan2()
        {
            double result = _mainPage.EvaluateExpression("10 > 15");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void LessThan1()
        { 
            double result = _mainPage.EvaluateExpression("5 < 10");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void LessThan2()
        { 
            double result = _mainPage.EvaluateExpression("15 < 10");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Equality()
        { 
            double result = _mainPage.EvaluateExpression("7 = 7");
            Assert.AreEqual(1, result);
        }

        [Test]
        public void Inequality()
        {
            double result = _mainPage.EvaluateExpression("7 = 6");
            Assert.AreEqual(0, result);
        }

        [Test]
        public void Increment()
        { 
            double result = _mainPage.EvaluateExpression("5++");
            Assert.AreEqual(6, result);
        }

        [Test]
        public void Decrement()
        {
            double result = _mainPage.EvaluateExpression("5--");
            Assert.AreEqual(4, result);
        }

        [Test]
        public void Pow()
        {
            double result = _mainPage.EvaluateExpression("2^3");
            Assert.AreEqual(8, result);
        }

        [Test]
        public void DivisionByZero()
        { 
            Assert.Throws<DivideByZeroException>(() => _mainPage.EvaluateExpression("10 / 0"));
        }

        [Test]
        public void CellReferences1()
        { 
            _mainPage.grid.Children.OfType<Entry>().First(c => Grid.GetRow(c) == 1 && Grid.GetColumn(c) == 1).Text = "10";
            _mainPage.grid.Children.OfType<Entry>().First(c => Grid.GetRow(c) == 1 && Grid.GetColumn(c) == 2).Text = "20";
            string expression = _mainPage.ReplaceCellReferences("A1 + B1");
            double result = _mainPage.EvaluateExpression(expression);
            Assert.AreEqual(30, result);
        }

        [Test]
        public void CellReferences2()
        {
            _mainPage.grid.Children.OfType<Entry>().First(c => Grid.GetRow(c) == 1 && Grid.GetColumn(c) == 1).Text = "10";
            string expression = _mainPage.ReplaceCellReferences("A1 + 5");
            double result = _mainPage.EvaluateExpression(expression);
            Assert.AreEqual(15, result);
        }
    }
}

using System;
using System.Collections.Generic;
using Game.Core.Tests.Doubles;
using NUnit.Framework;
using UnityEngine;

namespace Game.Core.Tests.Editor
{
    public sealed class SoundPoolValidationTests
    {
        private GameObject poolObject;
        private SoundPool pool;

        [SetUp]
        public void SetUp()
        {
            poolObject = new GameObject("Test Sound Pool");
            pool = poolObject.AddComponent<SoundPool>();
        }

        [TearDown]
        public void TearDown()
        {
            if (poolObject != null)
                UnityEngine.Object.DestroyImmediate(poolObject);
        }

        [Test]
        public void ValidatePoolSize_InvalidConfiguration_Throws()
        {
            AudioServiceTestFactory.SetField<List<AudioSource>>(pool, "_audioSources", null);
            Assert.Throws<InvalidOperationException>(() => pool.ValidatePoolSize());

            AudioServiceTestFactory.SetField(pool, "_audioSources", new List<AudioSource>());
            Assert.Throws<InvalidOperationException>(() => pool.ValidatePoolSize());

            AudioServiceTestFactory.SetField(
                pool,
                "_audioSources",
                new List<AudioSource> { null });
            Assert.Throws<InvalidOperationException>(() => pool.ValidatePoolSize());
        }
    }
}

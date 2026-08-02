using System;
using NUnit.Framework;

namespace Game.Core.Tests.Editor
{
    public class CompositeProgressTests
    {
        [Test]
        public void CompositeProgress_ReturnsAverage()
        {
            float result = -1f;

            var composite = new CompositeProgress(
                handler: value => result = value,
                autoEnableUpdate: true,
                subProgressFactory: h => new ImmediateProgress<float>(h));

            IProgress<float> first = composite.CreateSubProgress();
            IProgress<float> second = composite.CreateSubProgress();

            first.Report(0.5f);
            second.Report(1f);

            Assert.That(result, Is.EqualTo(0.75f).Within(0.001f));
        }

        [Test]
        public void CompositeProgress_NewOperationStartsAtZero()
        {
            float result = -1f;

            var composite = new CompositeProgress(
                handler: value => result = value,
                autoEnableUpdate: true,
                subProgressFactory: h => new ImmediateProgress<float>(h));

            IProgress<float> first = composite.CreateSubProgress();
            IProgress<float> second = composite.CreateSubProgress();

            first.Report(1f);

            Assert.That(result, Is.EqualTo(0.5f).Within(0.001f));
        }

        [Test]
        public void CompositeProgress_AppliesWeights()
        {
            float result = -1f;

            CompositeProgress composite = new(
                handler: value => result = value,
                autoEnableUpdate: true,
                subProgressFactory: handler => new ImmediateProgress<float>(handler));

            IProgress<float> first = composite.CreateSubProgress(weight: 1f);
            IProgress<float> second = composite.CreateSubProgress(weight: 3f);

            first.Report(1f);
            second.Report(0f);

            Assert.That(result, Is.EqualTo(0.25f).Within(0.001f));
        }
    }
}

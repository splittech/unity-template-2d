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
    }
}

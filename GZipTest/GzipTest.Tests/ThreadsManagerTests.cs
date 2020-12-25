using GZipTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace GZipUnitTests
{
    [TestClass]
    public class ThreadsManagerTests
    {
        [TestMethod]
        public void Start_OneSegmentExist_CallsGet3TimesAllOther1Time()
        {
            // arrange
            Mock<IOperationManager> operationManagerStub = new Mock<IOperationManager>();
            Mock<IQueueManager> queueManagerStub = new Mock<IQueueManager>();
            IThreadsManager threadsManager = new ThreadsManager(operationManagerStub.Object, queueManagerStub.Object, 2);
            operationManagerStub
                .SetupSequence(operationManager => operationManager.Get())
                .Returns((DataSegment)null)
                .Returns(new DataSegment(0L, 1));
            operationManagerStub
                .Setup(operationManager => operationManager.Operate(It.Is<DataSegment>(s=>s.Id==0L)))
                .Returns(
                    new DataSegment(0, 1) 
                    { 
                        Data = new byte[] { 0x55 } 
                    });
            operationManagerStub
                .Setup(operationManager => operationManager.Save(It.IsAny<DataSegment>()));
            queueManagerStub
                .Setup(queueManager => queueManager.Enqueue(It.IsAny<DataSegment>()));
            queueManagerStub
                .Setup(queueManager => queueManager.Dequeue())
                .Returns(
                    new DataSegment(0, 1) 
                    { 
                        Data = new byte[] { 0x55 } 
                    });
            queueManagerStub
                .SetupSequence(queueManager => queueManager.IsDone())
                .Returns(false)
                .Returns(true);

            // act
            threadsManager.Start();

            // assert
            operationManagerStub.Verify(operationManager => operationManager.Get(), Times.Exactly(3));
            operationManagerStub.Verify(operationManager => operationManager.Operate(It.IsAny<DataSegment>()), Times.Once);
            queueManagerStub.Verify(queueManager => queueManager.Enqueue(It.IsAny<DataSegment>()), Times.Once);
            queueManagerStub.Verify(queueManager => queueManager.Dequeue(), Times.Once);
            operationManagerStub.Verify(operationManager => operationManager.Save(It.IsAny<DataSegment>()), Times.Once);
        }

        [TestMethod]
        public void Start_4SegmentsExists_SavingInRightSequence()
        {
            // arrange
            Mock<IOperationManager> operationManagerStub = new Mock<IOperationManager>();
            IQueueManager queueManager = new QueueManager(4);
            string resultOutput = "";
            string expectedOutput = "0123";
            IThreadsManager threadsManager = new ThreadsManager(operationManagerStub.Object, queueManager, 4);
            operationManagerStub
                .SetupSequence(operationManager => operationManager.Get())
                .Returns(new DataSegment(3L, 1))
                .Returns(new DataSegment(0L, 1))
                .Returns(new DataSegment(2L, 1))
                .Returns(new DataSegment(1L, 1));
            operationManagerStub
                .Setup(operationManager => operationManager.Operate(It.Is<DataSegment>(s => s.Id == 0L)))
                .Returns(
                    new DataSegment(0L, 1)
                    {
                        Data = new byte[] { 0x00 } 
                    });
            operationManagerStub
                .Setup(operationManager => operationManager.Operate(It.Is<DataSegment>(s => s.Id == 1L)))
                .Returns(
                    new DataSegment(1L, 1) 
                    {
                        Data = new byte[] { 0x01 } 
                    });
            operationManagerStub
                .Setup(operationManager => operationManager.Operate(It.Is<DataSegment>(s => s.Id == 2L)))
                .Returns(
                    new DataSegment(2L, 1)
                    { 
                        Data = new byte[] { 0x02 } 
                    });
            operationManagerStub
                .Setup(operationManager => operationManager.Operate(It.Is<DataSegment>(s => s.Id == 3L)))
                .Returns(
                    new DataSegment(3L, 1) 
                    { 
                        Data = new byte[] { 0x03 } 
                    });
            operationManagerStub
                .Setup(operationManager => operationManager.Save(It.IsAny<DataSegment>()))
                .Callback<DataSegment>(bytes=> resultOutput += bytes.Data[0]);

            // act
            threadsManager.Start();

            // assert
            Assert.AreEqual(expectedOutput, resultOutput);
        }
    }
}
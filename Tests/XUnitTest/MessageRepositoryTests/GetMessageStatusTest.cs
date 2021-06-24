using ChatyChaty.Domain.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace XUnitTest.MessageRepositoryTests
{
    public class GetMessageStatusTest:MessageRepositoryTestBase
    {
        [Fact]
        public async Task OneMessage_ShouldReturnOne()
        {
            //Arragne
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id).MarkAsDelivered()).Entity;
            await dbContext.SaveChangesAsync();
            //Assert
            var result = await repository.GetStatusAsync(user1.Id);
            //Act
            Assert.Contains(result, r => r.Id == message1.Id);
        }

        [Fact]
        public async Task OneMessage_NotNewUpdate_ShouldReturnZero()
        {
            //Arragne
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id).MarkAsDelivered()).Entity;
            await dbContext.SaveChangesAsync();
            var date = DateTime.UtcNow + TimeSpan.FromMinutes(1);
            //Assert
            var result = await repository.GetStatusAsync(user1.Id, date);
            //Act
            Assert.DoesNotContain(result, r => r.Id == message1.Id);
        }

        [Fact]
        public async Task OneMessage_ReceivedMessage_ShouldReturnZero()
        {
            //Arragne
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user2.Id)).Entity;
            await dbContext.SaveChangesAsync();
            //Assert
            var result = await repository.GetStatusAsync(user1.Id);
            //Act
            Assert.DoesNotContain(result, r => r.Id == message1.Id);
        }

        [Fact]
        public async Task OneMessage_ReceivedDeliiveredMessage_ShouldReturnZero()
        {
            //Arragne
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user2.Id).MarkAsDelivered()).Entity;
            await dbContext.SaveChangesAsync();
            //Assert
            var result = await repository.GetStatusAsync(user1.Id);
            //Act
            Assert.DoesNotContain(result, r => r.Id == message1.Id);
        }

        [Fact]
        public async Task OneMessage_NoStatusUpdate_ShouldReturnZero()
        {
            //Arragne
            var message1 = dbContext.Messages.Add(new Message("some message", chatUser1AndUser2.Id, user1.Id)).Entity;
            await dbContext.SaveChangesAsync();
            //Assert
            var result = await repository.GetStatusAsync(user1.Id);
            //Act
            Assert.DoesNotContain(result, r => r.Id == message1.Id);
        }
    }
}

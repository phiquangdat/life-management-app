using Xunit;
using Moq;
using NoteTakingApp.ViewModels;
using NoteTakingApp.Services;
using NoteTakingApp.Models;
using System.Collections.ObjectModel;

namespace NoteTakingApp.Tests;

public class AllNotesViewModelTests
{
    [Fact]
    public async Task GetJokeCommand_UpdatesJokeOfTheDay_WhenServiceReturnsJoke()
    {
        // 1. ARRANGE
        var mockJokeService = new Mock<IJokeService>();
        var mockNoteService = new Mock<INoteService>();
        var mockSerializationService = new Mock<ISerializationService>();

        mockJokeService.Setup(s => s.GetProgrammingJokeAsync())
                       .ReturnsAsync("Why do Java developers wear glasses? Because they don't C#.");

        // NOTE: The constructor will trigger the FIRST call to GetProgrammingJokeAsync
        var viewModel = new AllNotesViewModel(
            mockJokeService.Object, 
            mockNoteService.Object,
            mockSerializationService.Object
        );

        // 2. ACT (Triggers the SECOND call)
        await viewModel.GetJokeCommand.ExecuteAsync(null);

        // 3. ASSERT
        Assert.Equal("Why do Java developers wear glasses? Because they don't C#.", viewModel.JokeOfTheDay);
        
        // FIX: Verify it was called exactly 2 times (Constructor + Command)
        mockJokeService.Verify(s => s.GetProgrammingJokeAsync(), Times.Exactly(2));
    }
}
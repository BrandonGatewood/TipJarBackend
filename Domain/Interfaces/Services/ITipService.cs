namespace TipJar.Domain.Interfaces.Services;

/// <summary>
/// Provides operations for managing user tips, including creating and updating tips.
/// Handles validation, persistence, and cache invalidation for tip-related actions.
/// </summary>
public interface ITipService
{   
    /// <summary>
    /// Creates a new tip for the currently authenticated user.
    /// </summary>
    /// <param name="amount">The tip amount to add.</param>
    /// <exception cref="InvalidTipAmountException">
    /// Thrown when the tip amount is less than or equal to zero.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the current user cannot be found.
    /// </exception>
    Task AddTipAsync(decimal amount);

    /// <summary>
    /// Updates an existing tip for the currently authenticated user.
    /// </summary>
    /// <param name="id">The identifier of the tip to update.</param>
    /// <param name="amount">The new tip amount.</param>
    /// <param name="createdAt">
    /// The tip date in MM/dd/yyyy format.
    /// </param>
    /// <exception cref="InvalidTipAmountException">
    /// Thrown when the tip amount is less than or equal to zero.
    /// </exception>
    /// <exception cref="InvalidDateFormatException">
    /// Thrown when the date is not in MM/dd/yyyy format.
    /// </exception>
    /// <exception cref="NotFoundException">
    /// Thrown when the current user cannot be found.
    /// </exception>
    Task EditTipAsync(Guid id, decimal amount, string createdAt);
}
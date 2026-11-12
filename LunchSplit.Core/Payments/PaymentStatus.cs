/*
 * FILE          : PaymentStatus.cs
 * PROJECT       : CSCN72010 Software Quality I - Assignment 3 (LunchSplit)
 * PROGRAMMER    : mrtobititiloye2748
 * FIRST VERSION : 2026-11-12
 * DESCRIPTION   :
 *   Domain enum for the life-cycle status of a payment. The gateway wrapper
 *   maps the vendor status text (for example Square's "COMPLETED") onto these
 *   values so Core code never compares raw vendor strings.
 */

namespace LunchSplit.Core.Payments
{
    // Possible states of a payment at the gateway.
    public enum PaymentStatus
    {
        Unknown,    // status text was missing or not recognised
        Approved,   // card authorised but not yet captured
        Pending,    // gateway is still processing
        Completed,  // money captured - Square status "COMPLETED"
        Canceled,   // payment was cancelled before capture
        Failed      // payment failed (declined, error, etc.)
    }
}

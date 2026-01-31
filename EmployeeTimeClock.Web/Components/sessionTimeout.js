// sessionTimeout.js - Manages inactivity detection and session timeout warnings

let dotnetRef = null;
let timeoutId = null;
let warningId = null;
let totalTimeoutSeconds = 0;
let warningTimeSeconds = 0;
let countdownSeconds = 0;

/**
 * Initialize session timeout monitoring
 * @param {DotNetObject} dotnetReference - Reference to C# SessionTimeoutService
 * @param {number} totalTimeout - Total timeout in seconds
 * @param {number} warningTime - Time to show warning in seconds  
 * @param {number} countdown - Time from warning to actual timeout in seconds
 */
export function initSessionTimeout(dotnetReference, totalTimeout, warningTime, countdown) {
    dotnetRef = dotnetReference;
    totalTimeoutSeconds = totalTimeout;
    warningTimeSeconds = warningTime;
    countdownSeconds = countdown;

    // Add global activity listeners
    ['mousedown', 'keydown', 'scroll', 'touchstart', 'click'].forEach(event => {
        document.addEventListener(event, () => resetSessionTimeout(), { passive: true });
    });

    // Start the timeout timer
    startSessionTimer();
}

/**
 * Reset the session timeout timer
 */
export function resetSessionTimeout() {
    if (timeoutId) clearTimeout(timeoutId);
    if (warningId) clearTimeout(warningId);

    startSessionTimer();
}

/**
 * Start the timer sequence
 */
function startSessionTimer() {
    // Set warning timer
    warningId = setTimeout(async () => {
        if (dotnetRef) {
            await dotnetRef.invokeMethodAsync('OnSessionWarning');
        }
        // Start countdown timer after warning
        startCountdownTimer();
    }, warningTimeSeconds * 1000);
}

/**
 * Start the countdown timer that leads to timeout
 */
function startCountdownTimer() {
    timeoutId = setTimeout(async () => {
        if (dotnetRef) {
            await dotnetRef.invokeMethodAsync('OnSessionTimeout');
        }
    }, countdownSeconds * 1000);
}

/**
 * Cleanup function
 */
export function cleanup() {
    if (timeoutId) clearTimeout(timeoutId);
    if (warningId) clearTimeout(warningId);
    dotnetRef = null;
}

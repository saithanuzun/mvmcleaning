// src/pages/PaymentFailedPage.jsx
import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const PaymentFailedPage = () => {
    const navigate = useNavigate();
    const [bookingId, setBookingId] = useState(null);

    useEffect(() => {
        // Get booking ID from localStorage
        const savedBookingId = localStorage.getItem('pending_booking_id');
        if (savedBookingId) {
            setBookingId(savedBookingId);
        }
    }, []);

    const handleRetryPayment = () => {
        // Go back to booking page to retry
        navigate('/payment');
    };

    const handleReturnHome = () => {
        // Clear localStorage and return home
        localStorage.removeItem('pending_booking_id');
        localStorage.removeItem('booking_session_id');
        localStorage.removeItem('booking_data');
        navigate('/postcode');
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 flex items-center justify-center px-4">
            <div className="max-w-md w-full">
                {/* Error Card */}
                <div className="bg-white rounded-2xl shadow-xl p-8">
                    <div className="text-center mb-6">
                        {/* Error Icon */}
                        <div className="inline-flex items-center justify-center w-20 h-20 rounded-full mb-6" style={{ backgroundColor: '#fee2e2' }}>
                            <svg className="w-10 h-10 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                            </svg>
                        </div>

                        {/* Error Message */}
                        <h1 className="text-3xl font-bold mb-2" style={{ color: '#dc2626' }}>
                            Payment Failed
                        </h1>
                        <p className="text-gray-600 text-lg mb-4">
                            We couldn't process your payment
                        </p>

                        {/* Booking Reference */}
                        {bookingId && (
                            <div className="bg-gray-50 p-4 rounded-lg mb-6 border-l-4" style={{ borderColor: '#dc2626' }}>
                                <p className="text-xs text-gray-500 mb-1 font-semibold uppercase">Booking Reference</p>
                                <p className="font-bold text-lg" style={{ color: '#dc2626' }}>
                                    {bookingId.substring(0, 8).toUpperCase()}
                                </p>
                            </div>
                        )}

                        {/* Error Details */}
                        <div className="bg-red-50 border-l-4 border-red-400 p-4 rounded-lg mb-6 text-left">
                            <p className="text-sm text-red-800 mb-2 font-semibold">What went wrong?</p>
                            <ul className="text-sm text-red-700 space-y-1 list-disc list-inside">
                                <li>Your card may have been declined</li>
                                <li>Insufficient funds in your account</li>
                                <li>Payment method expired</li>
                                <li>Network connection issue</li>
                            </ul>
                        </div>

                        {/* Help Text */}
                        <div className="bg-blue-50 border-l-4 border-blue-400 p-4 rounded-lg mb-6 text-left">
                            <p className="text-sm text-blue-800 font-semibold mb-2">What you can do:</p>
                            <ul className="text-sm text-blue-700 space-y-1">
                                <li>✓ Try again with the same card</li>
                                <li>✓ Use a different payment method</li>
                                <li>✓ Contact your bank for assistance</li>
                                <li>✓ Switch to Cash payment option</li>
                            </ul>
                        </div>
                    </div>

                    {/* Action Buttons */}
                    <div className="space-y-3">
                        <button
                            onClick={handleRetryPayment}
                            className="w-full px-6 py-3 text-white font-bold rounded-xl transition-all hover:shadow-lg"
                            style={{ backgroundColor: '#194376' }}
                        >
                            Retry Payment
                        </button>

                        <button
                            onClick={handleReturnHome}
                            className="w-full px-6 py-3 border-2 border-gray-300 text-gray-700 font-bold rounded-xl hover:bg-gray-50 transition-all"
                        >
                            Start Over
                        </button>
                    </div>

                    {/* Additional Help */}
                    <div className="mt-6 pt-6 border-t border-gray-200 text-center">
                        <p className="text-sm text-gray-600 mb-2">Need help?</p>
                        <a
                            href="mailto:support@example.com"
                            className="text-sm font-semibold hover:underline"
                            style={{ color: '#194376' }}
                        >
                            Contact Support
                        </a>
                    </div>
                </div>

                {/* Info Box */}
                <div className="mt-6 bg-blue-50 rounded-lg p-4 border-l-4" style={{ borderColor: '#46C6CE' }}>
                    <p className="text-sm text-gray-700">
                        <span className="font-semibold">Note:</span> Your booking is still reserved. You can retry payment anytime from the booking page.
                    </p>
                </div>
            </div>
        </div>
    );
};

export default PaymentFailedPage;

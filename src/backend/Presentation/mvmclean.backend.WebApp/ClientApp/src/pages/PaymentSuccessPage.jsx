// src/pages/PaymentSuccessPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate, useSearchParams } from 'react-router-dom';
import api from '../services/api';

const PaymentSuccessPage = () => {
    const [searchParams] = useSearchParams();
    const navigate = useNavigate();
    const [verifying, setVerifying] = useState(true);
    const [success, setSuccess] = useState(false);
    const [error, setError] = useState('');
    const [bookingDetails, setBookingDetails] = useState(null);

    useEffect(() => {
        const verifyPayment = async () => {
            try {
                // Get session ID from URL params (Stripe returns this)
                const sessionId = searchParams.get('session_id');
                const bookingId = localStorage.getItem('pending_booking_id');

                if (!sessionId || !bookingId) {
                    setError('Payment verification failed. Missing required information.');
                    setVerifying(false);
                    return;
                }

                // Verify payment with backend
                const response = await api.booking.verifyPayment(bookingId, sessionId);

                if (response.success) {
                    setSuccess(true);
                    setBookingDetails(response.data);

                    // Clear booking session
                    localStorage.removeItem('pending_booking_id');
                    localStorage.removeItem('booking_session_id');
                } else {
                    setError(response.message || 'Payment verification failed');
                }
            } catch (err) {
                console.error('Payment verification error:', err);
                setError('Failed to verify payment. Please contact support with your booking reference.');
            } finally {
                setVerifying(false);
            }
        };

        verifyPayment();
    }, [searchParams]);

    if (verifying) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 flex items-center justify-center px-4">
                <div className="text-center">
                    <div
                        className="animate-spin rounded-full h-16 w-16 border-4 border-t-transparent mx-auto mb-6"
                        style={{ borderColor: '#46C6CE', borderTopColor: 'transparent' }}
                    ></div>
                    <h2 className="text-2xl font-bold text-gray-800 mb-2">Verifying Payment...</h2>
                    <p className="text-gray-600">Please wait while we confirm your booking</p>
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 flex items-center justify-center px-4">
                <div className="max-w-md w-full bg-white rounded-2xl shadow-xl p-8">
                    <div className="text-center mb-6">
                        <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mx-auto mb-4">
                            <svg className="w-8 h-8 text-red-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                            </svg>
                        </div>
                        <h2 className="text-2xl font-bold text-gray-800 mb-2">Payment Verification Failed</h2>
                        <p className="text-gray-600 mb-6">{error}</p>
                        <button
                            onClick={() => navigate('/')}
                            className="px-8 py-3 bg-[#194376] text-white font-bold rounded-xl hover:shadow-lg transition-all"
                        >
                            Return to Home
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-12 px-4">
            <div className="max-w-3xl mx-auto">
                {/* Success Animation */}
                <div className="text-center mb-8">
                    <div className="inline-flex items-center justify-center w-20 h-20 rounded-full mb-6 animate-in zoom-in" style={{ backgroundColor: '#46C6CE' }}>
                        <svg className="w-10 h-10 text-white" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd" />
                        </svg>
                    </div>
                    <h1 className="text-4xl font-bold mb-2" style={{ color: '#194376' }}>
                        Booking Confirmed!
                    </h1>
                    <p className="text-gray-600 text-lg">
                        Your payment was successful and your booking is confirmed
                    </p>
                </div>

                {/* Booking Details Card */}
                <div className="bg-white rounded-2xl shadow-xl p-8 mb-6">
                    <div className="border-b-2 border-gray-100 pb-6 mb-6">
                        <h2 className="text-2xl font-bold text-gray-800 mb-2">Booking Details</h2>
                        <p className="text-gray-600">
                            A confirmation email has been sent to your email address
                        </p>
                    </div>

                    {bookingDetails && (
                        <div className="space-y-6">
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                                <div className="bg-gray-50 p-4 rounded-xl">
                                    <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">
                                        Booking Reference
                                    </p>
                                    <p className="font-bold text-xl" style={{ color: '#194376' }}>
                                        {bookingDetails.bookingId?.substring(0, 8).toUpperCase() || 'N/A'}
                                    </p>
                                </div>

                                <div className="bg-gray-50 p-4 rounded-xl">
                                    <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">
                                        Status
                                    </p>
                                    <div className="flex items-center">
                                        <div className="w-2 h-2 rounded-full bg-green-500 mr-2"></div>
                                        <p className="font-bold text-lg text-green-600">
                                            {bookingDetails.status || 'Confirmed'}
                                        </p>
                                    </div>
                                </div>
                            </div>

                            <div className="bg-gradient-to-r from-blue-50 to-cyan-50 p-6 rounded-xl border-2" style={{ borderColor: '#46C6CE40' }}>
                                <h3 className="font-bold text-lg text-gray-800 mb-4">What happens next?</h3>
                                <ul className="space-y-3">
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">1</span>
                                        </div>
                                        <p className="text-gray-700">
                                            You will receive a confirmation email with all booking details
                                        </p>
                                    </li>
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">2</span>
                                        </div>
                                        <p className="text-gray-700">
                                            Your assigned contractor will contact you before the appointment
                                        </p>
                                    </li>
                                    <li className="flex items-start">
                                        <div className="w-6 h-6 rounded-full flex items-center justify-center mr-3 flex-shrink-0 mt-0.5" style={{ backgroundColor: '#46C6CE' }}>
                                            <span className="text-white text-xs font-bold">3</span>
                                        </div>
                                        <p className="text-gray-700">
                                            We'll send you a reminder 24 hours before your scheduled service
                                        </p>
                                    </li>
                                </ul>
                            </div>
                        </div>
                    )}
                </div>

                {/* Action Buttons */}
                <div className="flex flex-col sm:flex-row gap-4 justify-center">
                    <button
                        onClick={() => navigate('/')}
                        className="px-8 py-4 bg-white border-2 border-gray-300 text-gray-700 font-bold rounded-xl hover:bg-gray-50 hover:border-gray-400 transition-all"
                    >
                        Back to Home
                    </button>
                    <button
                        onClick={() => navigate('/')}
                        className="px-8 py-4 bg-[#194376] text-white font-bold rounded-xl hover:shadow-xl transition-all transform hover:scale-105"
                    >
                        Book Another Service
                    </button>
                </div>

                {/* Support Info */}
                <div className="mt-8 text-center">
                    <p className="text-gray-600 text-sm">
                        Need help? Contact us at{' '}
                        <a href="mailto:support@mvmclean.com" className="font-semibold" style={{ color: '#194376' }}>
                            support@mvmclean.com
                        </a>
                    </p>
                </div>
            </div>
        </div>
    );
};

export default PaymentSuccessPage;

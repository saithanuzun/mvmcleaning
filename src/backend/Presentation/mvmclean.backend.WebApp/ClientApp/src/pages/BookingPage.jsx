// src/pages/PaymentPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const PaymentPage = ({ bookingData, updateBookingData }) => {
    const navigate = useNavigate();

    const [customerDetails, setCustomerDetails] = useState({
        name: '',
        email: '',
        address: '',
        city: '',
        postcode: bookingData.postcode || '',
        notes: ''
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState('');

    // Redirect if incomplete booking data
    useEffect(() => {
        if (!bookingData.postcode || !bookingData.basket || !bookingData.selectedTimeSlot) {
            navigate('/');
        }
    }, [bookingData, navigate]);

    const handleInputChange = (e) => {
        setCustomerDetails({
            ...customerDetails,
            [e.target.name]: e.target.value
        });
    };

    const validateForm = () => {
        if (!customerDetails.name.trim()) return 'Name is required';
        if (!customerDetails.email.trim()) return 'Email is required';
        if (!customerDetails.address.trim()) return 'Address is required';
        if (!customerDetails.city.trim()) return 'City is required';
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        if (!emailRegex.test(customerDetails.email)) return 'Invalid email address';
        return null;
    };

    const createDateTime = (date, timeString) => {
        // Parse time string (e.g., "09:00" or "09:00:00")
        const [hours, minutes] = timeString.split(':').map(Number);
        const dateTime = new Date(date);
        dateTime.setHours(hours, minutes, 0, 0);
        return dateTime.toISOString();
    };

    const handleSubmit = async (e) => {
        e.preventDefault();

        const validationError = validateForm();
        if (validationError) {
            setError(validationError);
            window.scrollTo({ top: 0, behavior: 'smooth' });
            return;
        }

        setLoading(true);
        setError('');

        try {
            // Prepare booking data for API
            const bookingRequest = {
                customerName: customerDetails.name,
                customerEmail: customerDetails.email,
                customerPhone: bookingData.phone,
                address: `${customerDetails.address}, ${customerDetails.city}`,
                postcode: bookingData.postcode,
                contractorId: bookingData.contractorId,
                scheduledSlot: {
                    startTime: createDateTime(bookingData.selectedDate, bookingData.selectedTimeSlot.startTime),
                    endTime: createDateTime(bookingData.selectedDate, bookingData.selectedTimeSlot.endTime)
                },
                services: bookingData.basket.items.map(item => ({
                    serviceId: item.serviceId,
                    serviceName: item.serviceName,
                    quantity: item.quantity,
                    price: item.price
                })),
                totalAmount: bookingData.totalAmount
            };

            // Add notes to address if provided
            if (customerDetails.notes) {
                bookingRequest.address += ` (Notes: ${customerDetails.notes})`;
            }

            // Create booking and get payment URL
            const response = await api.booking.create(bookingRequest);

            if (response.success && response.data) {
                // Store booking ID for payment verification
                localStorage.setItem('pending_booking_id', response.data.bookingId);

                // Update booking data
                updateBookingData({
                    customerDetails,
                    bookingId: response.data.bookingId
                });

                // Redirect to payment URL (Stripe Payment Link)
                window.location.href = response.data.paymentUrl;
            } else {
                setError(response.message || 'Failed to create booking. Please try again.');
            }
        } catch (err) {
            console.error('Booking creation error:', err);
            setError(err.response?.data?.message || 'Failed to create booking. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    const formatTime = (timeString) => {
        return new Date(`1970-01-01T${timeString}`).toLocaleTimeString('en-GB', {
            hour: '2-digit',
            minute: '2-digit',
            hour12: true
        });
    };

    const formatDateFull = (date) => {
        if (!date) return '';
        return date.toLocaleDateString('en-GB', {
            weekday: 'long',
            day: 'numeric',
            month: 'long',
            year: 'numeric'
        });
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-8 px-4">
            <div className="max-w-4xl mx-auto">
                {/* Header */}
                <div className="text-center mb-8">
                    <div className="inline-block px-4 py-2 rounded-full mb-4" style={{ backgroundColor: '#46C6CE20' }}>
                        <span className="font-bold text-sm" style={{ color: '#194376' }}>Step 4 of 4</span>
                    </div>
                    <h1 className="text-4xl font-bold mb-2" style={{ color: '#194376' }}>
                        Complete Your Booking
                    </h1>
                    <p className="text-gray-600 text-lg">
                        Enter your details to proceed to payment
                    </p>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="bg-red-50 border-l-4 border-red-500 rounded-lg p-4 mb-8 animate-in fade-in">
                        <div className="flex items-start">
                            <svg className="h-5 w-5 text-red-500 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <p className="text-red-700 font-medium">{error}</p>
                        </div>
                    </div>
                )}

                {/* Booking Summary */}
                <div className="bg-white rounded-2xl p-6 mb-8 border-2 shadow-lg" style={{ borderColor: '#46C6CE' }}>
                    <div className="flex items-center mb-4">
                        <div className="w-10 h-10 rounded-lg flex items-center justify-center mr-3" style={{ background: '#194376' }}>
                            <svg className="w-6 h-6 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2" />
                            </svg>
                        </div>
                        <h3 className="text-xl font-bold text-gray-800">Booking Summary</h3>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Services</p>
                            <p className="font-bold text-gray-800 text-sm leading-tight">
                                {bookingData.basket?.items?.map(item => item.serviceName).join(', ') || 'No services'}
                            </p>
                        </div>
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Date & Time</p>
                            <p className="font-bold text-gray-800 text-sm">
                                {bookingData.selectedDate && bookingData.selectedTimeSlot && (
                                    <>
                                        {formatDateFull(bookingData.selectedDate)}
                                        <br />
                                        {formatTime(bookingData.selectedTimeSlot.startTime)} - {formatTime(bookingData.selectedTimeSlot.endTime)}
                                    </>
                                )}
                            </p>
                        </div>
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Location</p>
                            <p className="font-bold text-gray-800">{bookingData.postcode}</p>
                            <p className="text-xs text-gray-600 mt-1">Phone: {bookingData.phone}</p>
                        </div>
                        <div className="bg-gray-50 p-4 rounded-xl">
                            <p className="text-xs text-gray-500 mb-1 font-semibold uppercase tracking-wide">Contractor</p>
                            <p className="font-bold text-gray-800">{bookingData.contractorName || 'Assigned'}</p>
                            <div className="mt-2">
                                <p className="text-xs text-gray-500 font-semibold uppercase tracking-wide">Total</p>
                                <p className="font-bold text-2xl" style={{ color: '#194376' }}>
                                    £{bookingData.totalAmount?.toFixed(2) || '0.00'}
                                </p>
                            </div>
                        </div>
                    </div>
                </div>

                <form onSubmit={handleSubmit} className="space-y-8">
                    {/* Customer Details Section */}
                    <div className="bg-white rounded-2xl shadow-lg p-6 border-2 border-gray-100">
                        <h3 className="text-2xl font-bold text-gray-800 mb-6 flex items-center">
                            <svg className="w-6 h-6 mr-2" style={{ color: '#194376' }} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                            </svg>
                            Your Details
                        </h3>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    Full Name *
                                </label>
                                <input
                                    type="text"
                                    name="name"
                                    value={customerDetails.name}
                                    onChange={handleInputChange}
                                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all"
                                    placeholder="John Smith"
                                    required
                                />
                            </div>

                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    Email Address *
                                </label>
                                <input
                                    type="email"
                                    name="email"
                                    value={customerDetails.email}
                                    onChange={handleInputChange}
                                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all"
                                    placeholder="john@example.com"
                                    required
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    Address *
                                </label>
                                <input
                                    type="text"
                                    name="address"
                                    value={customerDetails.address}
                                    onChange={handleInputChange}
                                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all"
                                    placeholder="123 Main Street"
                                    required
                                />
                            </div>

                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    City *
                                </label>
                                <input
                                    type="text"
                                    name="city"
                                    value={customerDetails.city}
                                    onChange={handleInputChange}
                                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all"
                                    placeholder="London"
                                    required
                                />
                            </div>
                        </div>

                        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    Postcode
                                </label>
                                <input
                                    type="text"
                                    name="postcode"
                                    value={customerDetails.postcode}
                                    className="w-full px-4 py-3 border-2 border-gray-200 rounded-xl bg-gray-50 text-gray-600"
                                    readOnly
                                />
                            </div>

                            <div>
                                <label className="block text-gray-700 text-sm font-bold mb-2">
                                    Special Instructions (Optional)
                                </label>
                                <textarea
                                    name="notes"
                                    value={customerDetails.notes}
                                    onChange={handleInputChange}
                                    className="w-full px-4 py-3 border-2 border-gray-300 rounded-xl focus:border-[#46C6CE] focus:ring-2 focus:ring-[#46C6CE]/20 outline-none transition-all"
                                    placeholder="Any special instructions..."
                                    rows="3"
                                />
                            </div>
                        </div>
                    </div>

                    {/* Terms and Conditions */}
                    <div className="bg-gradient-to-r from-blue-50 to-cyan-50 rounded-xl p-6 border-2" style={{ borderColor: '#46C6CE40' }}>
                        <div className="flex items-start">
                            <input
                                type="checkbox"
                                id="terms"
                                required
                                className="mt-1 mr-3 h-5 w-5 rounded border-2 focus:ring-2 focus:ring-offset-2"
                                style={{ accentColor: '#194376', borderColor: '#194376' }}
                            />
                            <label htmlFor="terms" className="text-sm text-gray-700 leading-relaxed">
                                I agree to the Terms of Service and understand that this booking is subject to availability confirmation.
                                I authorize the charge for the selected services and understand that payment will be processed securely via Stripe.
                            </label>
                        </div>
                    </div>

                    {/* Navigation Buttons */}
                    <div className="flex flex-col sm:flex-row justify-between items-center gap-4 pt-6 border-t-2 border-gray-200">
                        <button
                            type="button"
                            onClick={() => navigate('/time-slots')}
                            className="px-8 py-4 border-2 border-gray-300 text-gray-700 font-bold rounded-xl hover:bg-gray-50 hover:border-gray-400 transition-all w-full sm:w-auto flex items-center justify-center gap-2"
                        >
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 19l-7-7 7-7" />
                            </svg>
                            Back to Time Slots
                        </button>

                        <button
                            type="submit"
                            disabled={loading}
                            className={`
                                px-8 py-4 font-bold rounded-xl transition-all w-full sm:w-auto flex items-center justify-center gap-2
                                ${loading
                                    ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                                    : 'bg-[#194376] text-white hover:shadow-2xl transform hover:scale-105 active:scale-95'
                                }
                            `}
                        >
                            {loading ? (
                                <>
                                    <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                    </svg>
                                    Creating Booking...
                                </>
                            ) : (
                                <>
                                    Proceed to Payment
                                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M13 7l5 5m0 0l-5 5m5-5H6" />
                                    </svg>
                                </>
                            )}
                        </button>
                    </div>
                </form>

                {/* Payment Info */}
                <div className="mt-6 text-center">
                    <div className="inline-flex items-center gap-2 text-sm text-gray-600">
                        <svg className="w-4 h-4 text-[#46C6CE]" fill="currentColor" viewBox="0 0 20 20">
                            <path fillRule="evenodd" d="M5 9V7a5 5 0 0110 0v2a2 2 0 012 2v5a2 2 0 01-2 2H5a2 2 0 01-2-2v-5a2 2 0 012-2zm8-2v2H7V7a3 3 0 016 0z" clipRule="evenodd" />
                        </svg>
                        Secure payment powered by Stripe
                    </div>
                </div>
            </div>
        </div>
    );
};

export default PaymentPage;

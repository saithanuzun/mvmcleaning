// src/pages/PaymentPage.jsx
import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { loadStripe } from '@stripe/stripe-js';
import { Elements, CardElement, useStripe, useElements } from '@stripe/react-stripe-js';
import axios from 'axios';

const stripePromise = loadStripe('pk_test_51YourTestKeyHere'); // Replace with your Stripe publishable key

const PaymentForm = ({ bookingData, updateBookingData }) => {
    const stripe = useStripe();
    const elements = useElements();
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

    const handleSubmit = async (e) => {
        e.preventDefault();

        const validationError = validateForm();
        if (validationError) {
            setError(validationError);
            return;
        }

        if (!stripe || !elements) {
            return;
        }

        setLoading(true);
        setError('');

        try {
            // Update booking data with customer details
            updateBookingData({ customerDetails });

            // 1. Create payment intent on your backend
            const paymentIntentResponse = await axios.post('http://localhost:5000/api/create-payment-intent', {
                amount: Math.round(bookingData.totalAmount * 100), // Convert to pence
                currency: 'gbp',
                customerDetails,
                bookingDetails: {
                    postcode: bookingData.postcode,
                    phone: bookingData.phone,
                    services: bookingData.selectedServicesData,
                    timeSlot: bookingData.selectedTimeSlot
                }
            });

            const { clientSecret } = paymentIntentResponse.data;

            // 2. Get card element
            const cardElement = elements.getElement(CardElement);

            // 3. Confirm card payment
            const { error: stripeError, paymentIntent } = await stripe.confirmCardPayment(clientSecret, {
                payment_method: {
                    card: cardElement,
                    billing_details: {
                        name: customerDetails.name,
                        email: customerDetails.email,
                        address: {
                            line1: customerDetails.address,
                            city: customerDetails.city,
                            postal_code: customerDetails.postcode
                        }
                    }
                }
            });

            if (stripeError) {
                setError(stripeError.message);
                setLoading(false);
                return;
            }

            if (paymentIntent.status === 'succeeded') {
                // 4. Save booking to your backend
                await axios.post('http://localhost:5000/api/create-booking', {
                    paymentIntentId: paymentIntent.id,
                    customerDetails,
                    bookingDetails: {
                        postcode: bookingData.postcode,
                        phone: bookingData.phone,
                        services: bookingData.selectedServicesData,
                        timeSlot: bookingData.selectedTimeSlot,
                        totalAmount: bookingData.totalAmount
                    }
                });

                // 5. Redirect to confirmation page
                navigate('/booking-confirmation', {
                    state: {
                        bookingId: paymentIntent.id,
                        customerName: customerDetails.name,
                        bookingDetails: bookingData
                    }
                });
            }
        } catch (err) {
            setError(err.response?.data?.message || 'Payment failed. Please try again.');
            console.error(err);
        } finally {
            setLoading(false);
        }
    };

    // Stripe Card Element styles
    const cardElementOptions = {
        style: {
            base: {
                fontSize: '16px',
                color: '#424770',
                '::placeholder': {
                    color: '#aab7c4',
                },
                padding: '10px',
            },
            invalid: {
                color: '#9e2146',
            },
        },
        hidePostalCode: true,
    };

    return (
        <div className="max-w-4xl mx-auto px-4 py-8">
            <h1 className="text-3xl font-bold text-gray-800 mb-2 text-center">
                Complete Your Booking
            </h1>
            <p className="text-gray-600 mb-8 text-center">
                Enter your details and payment information
            </p>

            {/* Booking Summary */}
            <div className="bg-blue-50 rounded-2xl p-6 mb-8 border border-blue-100">
                <h3 className="text-xl font-bold text-gray-800 mb-4">Booking Summary</h3>
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                        <p className="text-sm text-gray-500 mb-1">Services</p>
                        <p className="font-semibold text-gray-800">
                            {bookingData.selectedServicesData?.map(s => s.name).join(', ')}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-gray-500 mb-1">Date & Time</p>
                        <p className="font-semibold text-gray-800">
                            {bookingData.selectedTimeSlot && (
                                <>
                                    {new Date(bookingData.selectedTimeSlot.date).toLocaleDateString('en-GB', {
                                        weekday: 'long',
                                        day: 'numeric',
                                        month: 'long'
                                    })} at {bookingData.selectedTimeSlot.startTime}
                                </>
                            )}
                        </p>
                    </div>
                    <div>
                        <p className="text-sm text-gray-500 mb-1">Location</p>
                        <p className="font-semibold text-gray-800">{bookingData.postcode}</p>
                    </div>
                    <div className="text-right">
                        <p className="text-sm text-gray-500 mb-1">Total Amount</p>
                        <p className="font-bold text-3xl" style={{ color: '#194376' }}>
                            £{bookingData.totalAmount?.toFixed(2)}
                        </p>
                    </div>
                </div>
            </div>

            <form onSubmit={handleSubmit} className="space-y-8">
                {/* Customer Details Section */}
                <div className="bg-white rounded-2xl shadow-lg p-6 border border-gray-200">
                    <h3 className="text-xl font-bold text-gray-800 mb-6 flex items-center">
                        <svg className="w-6 h-6 mr-2" style={{ color: '#194376' }} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                        </svg>
                        Customer Details
                    </h3>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                Full Name *
                            </label>
                            <input
                                type="text"
                                name="name"
                                value={customerDetails.name}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:border-transparent outline-none transition"
                                style={{ '--tw-ring-color': '#194376' }}
                                placeholder="John Smith"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                Email Address *
                            </label>
                            <input
                                type="email"
                                name="email"
                                value={customerDetails.email}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:border-transparent outline-none transition"
                                style={{ '--tw-ring-color': '#194376' }}
                                placeholder="john@example.com"
                                required
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6 mb-6">
                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                Address *
                            </label>
                            <input
                                type="text"
                                name="address"
                                value={customerDetails.address}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:border-transparent outline-none transition"
                                style={{ '--tw-ring-color': '#194376' }}
                                placeholder="123 Main Street"
                                required
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                City *
                            </label>
                            <input
                                type="text"
                                name="city"
                                value={customerDetails.city}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:border-transparent outline-none transition"
                                style={{ '--tw-ring-color': '#194376' }}
                                placeholder="London"
                                required
                            />
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                Postcode
                            </label>
                            <input
                                type="text"
                                name="postcode"
                                value={customerDetails.postcode}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg bg-gray-50"
                                readOnly
                            />
                        </div>

                        <div>
                            <label className="block text-gray-700 text-sm font-semibold mb-2">
                                Special Instructions (Optional)
                            </label>
                            <textarea
                                name="notes"
                                value={customerDetails.notes}
                                onChange={handleInputChange}
                                className="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:border-transparent outline-none transition"
                                style={{ '--tw-ring-color': '#194376' }}
                                placeholder="Any special instructions for our team..."
                                rows="3"
                            />
                        </div>
                    </div>
                </div>

                {/* Payment Details Section */}
                <div className="bg-white rounded-2xl shadow-lg p-6 border border-gray-200">
                    <h3 className="text-xl font-bold text-gray-800 mb-6 flex items-center">
                        <svg className="w-6 h-6 mr-2" style={{ color: '#194376' }} fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 10h18M7 15h1m4 0h1m-7 4h12a3 3 0 003-3V8a3 3 0 00-3-3H6a3 3 0 00-3 3v8a3 3 0 003 3z" />
                        </svg>
                        Payment Details
                    </h3>

                    <div className="mb-6">
                        <label className="block text-gray-700 text-sm font-semibold mb-2">
                            Card Information *
                        </label>
                        <div className="border border-gray-300 rounded-lg p-4 focus-within:ring-2 focus-within:border-transparent"
                             style={{ '--tw-ring-color': '#194376' }}>
                            <CardElement options={cardElementOptions} />
                        </div>
                        <p className="text-xs text-gray-500 mt-2">
                            Your payment is secure and encrypted
                        </p>
                    </div>

                    {/* Test Card Info */}
                    <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 mb-6">
                        <div className="flex items-start">
                            <svg className="w-5 h-5 text-blue-600 mt-0.5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <div>
                                <p className="text-sm font-medium text-blue-800 mb-1">Test Mode</p>
                                <p className="text-xs text-blue-700">
                                    Use test card: <code className="bg-blue-100 px-2 py-1 rounded">4242 4242 4242 4242</code>
                                </p>
                                <p className="text-xs text-blue-700 mt-1">
                                    Any future expiry date, any 3-digit CVC
                                </p>
                            </div>
                        </div>
                    </div>

                    {/* Terms and Conditions */}
                    <div className="bg-gray-50 rounded-lg p-4 mb-6">
                        <div className="flex items-start">
                            <input
                                type="checkbox"
                                id="terms"
                                required
                                className="mt-1 mr-3 h-5 w-5 rounded focus:ring-0"
                                style={{
                                    accentColor: '#194376',
                                    '--tw-ring-color': '#194376'
                                }}
                            />
                            <label htmlFor="terms" className="text-sm text-gray-700">
                                I agree to the Terms of Service and understand that this booking is subject to
                                availability confirmation. I authorize the charge of £{bookingData.totalAmount?.toFixed(2)}
                                to my card for the selected services.
                            </label>
                        </div>
                    </div>
                </div>

                {/* Error Message */}
                {error && (
                    <div className="bg-red-50 border-l-4 border-red-500 p-4 rounded">
                        <div className="flex">
                            <svg className="h-5 w-5 text-red-500 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                            <p className="text-red-700">{error}</p>
                        </div>
                    </div>
                )}

                {/* Navigation Buttons */}
                <div className="flex flex-col sm:flex-row justify-between items-center pt-8 border-t border-gray-200 gap-4">
                    <button
                        type="button"
                        onClick={() => navigate('/time-slots')}
                        className="px-8 py-3 border-2 border-gray-300 text-gray-700 font-semibold rounded-lg hover:bg-gray-50 transition flex items-center"
                    >
                        <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                        </svg>
                        Back to Time Slots
                    </button>

                    <button
                        type="submit"
                        disabled={loading || !stripe}
                        className={`
              px-8 py-3 font-semibold rounded-lg transition flex items-center
              ${loading || !stripe
                            ? 'bg-gray-300 text-gray-500 cursor-not-allowed'
                            : 'text-white transform hover:-translate-y-0.5 shadow-lg hover:shadow-xl'
                        }
            `}
                        style={!loading && stripe ? { backgroundColor: '#194376' } : {}}
                    >
                        {loading ? (
                            <>
                                <svg className="animate-spin -ml-1 mr-3 h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                </svg>
                                Processing Payment...
                            </>
                        ) : (
                            <>
                                Pay £{bookingData.totalAmount?.toFixed(2)}
                                <svg className="w-5 h-5 ml-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M14 5l7 7m0 0l-7 7m7-7H3" />
                                </svg>
                            </>
                        )}
                    </button>
                </div>
            </form>
        </div>
    );
};

const PaymentPage = ({ bookingData, updateBookingData }) => {
    return (
        <Elements stripe={stripePromise}>
            <PaymentForm bookingData={bookingData} updateBookingData={updateBookingData} />
        </Elements>
    );
};

export default PaymentPage;
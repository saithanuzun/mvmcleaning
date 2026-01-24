// src/App.js
import React, { useState, useEffect } from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate, useLocation } from 'react-router-dom';
import PostcodePage from './pages/PostcodePage';
import ServicesPage from './pages/ServicesPage';
import TimeSlotsPage from './pages/TimeSlotsPage';
import BookingPage from './pages/BookingPage';
import PaymentSuccessPage from './pages/PaymentSuccessPage';
import PaymentFailedPage from './pages/PaymentFailedPage';
import ProgressBar from './components/ProgressBar';

function AppContent() {
    const [bookingData, setBookingData] = useState({
        postcode: '',
        phone: '',
        bookingId: null,
        selectedServices: [],
        selectedTimeSlot: null,
        customerDetails: {},
        totalAmount: 0
    });

    const location = useLocation();

    const resetBookingData = () => {
        setBookingData({
            postcode: '',
            phone: '',
            bookingId: null,
            selectedServices: [],
            selectedTimeSlot: null,
            customerDetails: {},
            totalAmount: 0
        });
    };

    // Reset data when user visits postcode page
    useEffect(() => {
        if (location.pathname === '/shop/postcode') {
            resetBookingData();
        }
    }, [location.pathname]);

    const updateBookingData = (newData) => {
        setBookingData(prev => ({ ...prev, ...newData }));
    };

    return (
        <div className="min-h-screen bg-gray-50">
            <ProgressBar />
            <div className="container mx-auto px-4 py-8">
                <Routes>
                    <Route path="/" element={<Navigate to="/postcode" />} />
                    <Route
                        path="/postcode"
                        element={<PostcodePage bookingData={bookingData} updateBookingData={updateBookingData} />}
                    />
                    <Route
                        path="/services"
                        element={<ServicesPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                    />
                    <Route
                        path="/time-slots"
                        element={<TimeSlotsPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                    />
                    <Route
                        path="/payment"
                        element={<BookingPage bookingData={bookingData} updateBookingData={updateBookingData} />}
                    />
                    <Route
                        path="/payment-success"
                        element={<PaymentSuccessPage />}
                    />
                    <Route
                        path="/payment-failed"
                        element={<PaymentFailedPage />}
                    />
                </Routes>
            </div>
        </div>
    );
}

function App() {
    return (
        <Router basename="/shop">
            <AppContent />
        </Router>
    );
}

export default App;
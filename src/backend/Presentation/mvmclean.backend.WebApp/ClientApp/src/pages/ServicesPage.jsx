// src/pages/ServicesPage.jsx
import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';

const ServicesPage = ({ bookingData, updateBookingData }) => {
    const [services, setServices] = useState([]);
    const [basket, setBasket] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');
    const [activeCategory, setActiveCategory] = useState('all');
    const [categories, setCategories] = useState([]);
    const navigate = useNavigate();

    // Redirect if no postcode
    useEffect(() => {
        if (!bookingData.postcode) {
            navigate('/postcode');
        }
    }, [bookingData.postcode, navigate]);

    // Fetch services by postcode
    useEffect(() => {
        const fetchServices = async () => {
            try {
                setLoading(true);
                const response = await api.services.getByPostcode(bookingData.postcode);

                if (response.success && response.data) {
                    setServices(response.data);

                    // Extract unique categories
                    const uniqueCategories = [...new Set(response.data.map(s => s.category))];
                    const categoryList = [
                        { id: 'all', name: 'All Services', color: '#46C6CE' },
                        ...uniqueCategories.map(cat => ({
                            id: cat.toLowerCase(),
                            name: cat,
                            color: '#46C6CE'
                        }))
                    ];
                    setCategories(categoryList);
                }
            } catch (err) {
                console.error('Error fetching services:', err);
                setError('Failed to load services. Please try again.');
            } finally {
                setLoading(false);
            }
        };

        if (bookingData.postcode) {
            fetchServices();
        }
    }, [bookingData.postcode]);

    // Load basket on mount
    useEffect(() => {
        const loadBasket = async () => {
            try {
                const response = await api.basket.get();
                if (response.success) {
                    setBasket(response.data);
                }
            } catch (err) {
                console.error('Error loading basket:', err);
            }
        };
        loadBasket();
    }, []);

    const handleAddService = async (service) => {
        try {
            setError('');
            const response = await api.basket.add(
                service.id,
                service.name,
                service.basePrice,
                service.duration
            );

            if (response.success) {
                setBasket(response.data);
            }
        } catch (err) {
            console.error('Error adding to basket:', err);
            setError('Failed to add service. Please try again.');
        }
    };

    const handleQuantityChange = async (serviceId, newQuantity) => {
        try {
            setError('');

            if (newQuantity === 0) {
                const response = await api.basket.remove(serviceId);
                if (response.success) {
                    setBasket(response.data);
                }
            } else {
                const response = await api.basket.updateQuantity(serviceId, newQuantity);
                if (response.success) {
                    setBasket(response.data);
                }
            }
        } catch (err) {
            console.error('Error updating quantity:', err);
            setError('Failed to update quantity. Please try again.');
        }
    };

    const getServiceQuantity = (serviceId) => {
        if (!basket || !basket.items) return 0;
        const item = basket.items.find(i => i.serviceId === serviceId);
        return item ? item.quantity : 0;
    };

    const getFilteredServices = () => {
        if (activeCategory === 'all') return services;
        return services.filter(service =>
            service.category.toLowerCase() === activeCategory.toLowerCase()
        );
    };

    const handleContinue = () => {
        if (!basket || !basket.items || basket.items.length === 0) {
            setError('Please add at least one service to continue');
            window.scrollTo({ top: 0, behavior: 'smooth' });
            return;
        }

        updateBookingData({
            basket: basket,
            selectedServices: basket.items,
            totalAmount: basket.totalAmount,
            totalDuration: basket.totalDuration
        });
        navigate('/time-slots');
    };

    if (loading) {
        return (
            <div className="flex justify-center items-center min-h-[60vh]">
                <div className="text-center">
                    <div
                        className="animate-spin rounded-full h-16 w-16 border-4 border-t-transparent mx-auto"
                        style={{ borderColor: '#46C6CE', borderTopColor: 'transparent' }}
                    ></div>
                    <p className="mt-6 text-gray-600 font-medium text-lg">Loading services...</p>
                </div>
            </div>
        );
    }

    if (services.length === 0) {
        return (
            <div className="flex justify-center items-center min-h-[60vh]">
                <div className="text-center max-w-md">
                    <div className="text-6xl mb-4">📦</div>
                    <h2 className="text-2xl font-bold text-gray-800 mb-2">No Services Available</h2>
                    <p className="text-gray-600 mb-6">We couldn't find any services for your area at the moment.</p>
                    <button
                        onClick={() => navigate('/postcode')}
                        className="px-6 py-3 text-white font-semibold rounded-lg"
                        style={{ background: '#194376' }}
                    >
                        Try Different Postcode
                    </button>
                </div>
            </div>
        );
    }

    return (
        <div className="min-h-screen bg-gradient-to-br from-gray-50 via-white to-blue-50/30 py-8 px-4">
            <div className="max-w-7xl mx-auto">


                {/* Error Message */}
                {error && (
                    <div className="mb-6 bg-red-50 border-l-4 border-red-500 p-4 rounded-lg animate-in fade-in">
                        <div className="flex items-start">
                            <svg className="h-5 w-5 text-red-500 mr-3 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                            </svg>
                            <p className="text-red-700 font-medium">{error}</p>
                        </div>
                    </div>
                )}

                {/* Header */}
                <div className="bg-white rounded-2xl shadow-lg p-8 mb-8">
                    <div className="flex flex-col md:flex-row md:items-center md:justify-between">
                        <div>
                            <div className="inline-block px-3 py-1 rounded-full mb-2" style={{ backgroundColor: '#46C6CE20' }}>
                                <span className="text-sm font-semibold" style={{ color: '#194376' }}>Step 2 of 4</span>
                            </div>
                            <h1 className="text-3xl font-bold mb-2" style={{ color: '#194376' }}>Choose Your Services</h1>
                            <p className="text-gray-600">Select the services you need for <span className="font-semibold">{bookingData.postcode}</span></p>
                        </div>

                        {/* Basket Summary */}
                        {basket && basket.items && basket.items.length > 0 && (
                            <div className="mt-4 md:mt-0 bg-gradient-to-br from-blue-50 to-cyan-50 rounded-xl p-4 border-2" style={{ borderColor: '#46C6CE40' }}>
                                <div className="text-sm text-gray-600 mb-1">Your Basket</div>
                                <div className="text-2xl font-bold" style={{ color: '#194376' }}>
                                    £{basket.totalAmount.toFixed(2)}
                                </div>
                                <div className="text-sm text-gray-600">
                                    {basket.items.length} service{basket.items.length !== 1 ? 's' : ''} • {basket.totalDuration} mins
                                </div>
                            </div>
                        )}
                    </div>
                </div>

                {/* Category Filter */}
                <div className="mb-8 flex flex-wrap gap-3">
                    {categories.map(category => (
                        <button
                            key={category.id}
                            onClick={() => setActiveCategory(category.id)}
                            className={`px-6 py-3 rounded-xl font-semibold transition-all duration-200 ${
                                activeCategory === category.id
                                    ? 'text-white shadow-lg transform scale-105'
                                    : 'bg-white text-gray-700 hover:shadow-md border-2 border-gray-200'
                            }`}
                            style={activeCategory === category.id ? {
                                background: 'linear-gradient(135deg, #194376 0%, #14325e 100%)'
                            } : {}}
                        >
                            {category.name}
                        </button>
                    ))}
                </div>

                {/* Services Grid */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6 mb-8">
                    {getFilteredServices().map(service => {
                        const quantity = getServiceQuantity(service.id);
                        const isSelected = quantity > 0;

                        return (
                            <div
                                key={service.id}
                                className={`bg-white rounded-2xl shadow-md hover:shadow-xl transition-all duration-300 overflow-hidden border-2 ${
                                    isSelected ? 'ring-2 ring-offset-2' : 'border-transparent'
                                }`}
                                style={isSelected ? { ringColor: '#46C6CE' } : {}}
                            >
                                <div className="p-6">
                                    <div className="flex justify-between items-start mb-3">
                                        <h3 className="text-xl font-bold text-gray-900">{service.name}</h3>
                                        {isSelected && (
                                            <div className="w-8 h-8 rounded-full flex items-center justify-center" style={{ backgroundColor: '#46C6CE' }}>
                                                <svg className="w-5 h-5 text-white" fill="currentColor" viewBox="0 0 20 20">
                                                    <path fillRule="evenodd" d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" clipRule="evenodd"/>
                                                </svg>
                                            </div>
                                        )}
                                    </div>

                                    <p className="text-gray-600 text-sm mb-4 line-clamp-2">{service.description}</p>

                                    <div className="flex items-center justify-between mb-4">
                                        <div>
                                            <div className="text-2xl font-bold" style={{ color: '#194376' }}>
                                                £{service.basePrice.toFixed(2)}
                                            </div>
                                            <div className="text-xs text-gray-500">{service.duration} minutes</div>
                                        </div>
                                        <div className="text-right">
                                            <div className="text-xs text-gray-500 mb-1">Category</div>
                                            <span className="text-xs font-semibold px-2 py-1 rounded-full" style={{ backgroundColor: '#46C6CE20', color: '#194376' }}>
                                                {service.category}
                                            </span>
                                        </div>
                                    </div>

                                    {/* Quantity Controls */}
                                    {isSelected ? (
                                        <div className="flex items-center justify-between bg-gray-50 rounded-lg p-2">
                                            <button
                                                onClick={() => handleQuantityChange(service.id, quantity - 1)}
                                                className="w-10 h-10 rounded-lg bg-white border-2 border-gray-300 flex items-center justify-center hover:border-red-400 hover:text-red-500 transition-colors"
                                            >
                                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M20 12H4"/>
                                                </svg>
                                            </button>
                                            <span className="text-xl font-bold" style={{ color: '#194376' }}>{quantity}</span>
                                            <button
                                                onClick={() => handleQuantityChange(service.id, quantity + 1)}
                                                className="w-10 h-10 rounded-lg flex items-center justify-center text-white hover:opacity-90 transition-opacity"
                                                style={{ background: '#46C6CE' }}
                                            >
                                                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M12 4v16m8-8H4"/>
                                                </svg>
                                            </button>
                                        </div>
                                    ) : (
                                        <button
                                            onClick={() => handleAddService(service)}
                                            className="w-full text-white font-bold py-3 px-4 rounded-xl transition-all duration-200 hover:shadow-lg hover:scale-105"
                                            style={{ background: 'linear-gradient(135deg, #194376 0%, #14325e 100%)' }}
                                        >
                                            Add to Basket
                                        </button>
                                    )}
                                </div>
                            </div>
                        );
                    })}
                </div>

                {/* Continue Button */}
                <div className="sticky bottom-0 bg-white border-t-4 p-6 rounded-t-3xl shadow-2xl" style={{ borderColor: '#46C6CE' }}>
                    <div className="max-w-4xl mx-auto flex flex-col md:flex-row items-center justify-between gap-4">
                        <div className="text-center md:text-left">
                            {basket && basket.items && basket.items.length > 0 ? (
                                <>
                                    <div className="text-sm text-gray-600">Total Amount</div>
                                    <div className="text-3xl font-bold" style={{ color: '#194376' }}>
                                        £{basket.totalAmount.toFixed(2)}
                                    </div>
                                    <div className="text-sm text-gray-600">
                                        {basket.items.length} service{basket.items.length !== 1 ? 's' : ''} • Approx {basket.totalDuration} minutes
                                    </div>
                                </>
                            ) : (
                                <div className="text-gray-500">No services selected</div>
                            )}
                        </div>

                        <button
                            onClick={handleContinue}
                            disabled={!basket || !basket.items || basket.items.length === 0}
                            className={`px-8 py-4 text-white font-bold rounded-xl transition-all duration-300 ${
                                basket && basket.items && basket.items.length > 0
                                    ? 'hover:shadow-xl hover:scale-105'
                                    : 'opacity-50 cursor-not-allowed'
                            }`}
                            style={{
                                background: basket && basket.items && basket.items.length > 0
                                    ? 'linear-gradient(135deg, #194376 0%, #14325e 100%)'
                                    : '#9ca3af'
                            }}
                        >
                            <span className="flex items-center">
                                Continue to Time Selection
                                <svg className="ml-2 w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2.5" d="M13 7l5 5m0 0l-5 5m5-5H6"/>
                                </svg>
                            </span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ServicesPage;

import React, { useEffect, useState } from 'react';
import { useGlobalContext } from '../context';
import { CustomButton, GameLoad, PageHOC } from '../components';
import styles from '../styles';

const EquipmentPage = () => {
  const { walletAddress } = useGlobalContext();
  const [shopEquipments, setShopEquipments] = useState([]); 
  const [ownedEquipments, setOwnedEquipments] = useState([]);
  const [balance, setBalance] = useState(0);
  const [isLoading, setIsLoading] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  useEffect(() => {
    if (walletAddress) {
      fetchData();
    }
  }, [walletAddress]);

  const fetchData = async () => {
    try {
      setIsLoading(true);
      await Promise.all([
        fetchShopEquipments(),
        fetchOwnedEquipments(),
        fetchBalance()
      ]);
    } catch (error) {
      setErrorMessage(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchShopEquipments = async () => {
    const response = await fetch('/api/equipment');
    if (!response.ok) {
      throw new Error('Failed to fetch shop equipments');
    }
    const data = await response.json();
    setShopEquipments(data);
  };

  const fetchOwnedEquipments = async () => {
    const response = await fetch(`/api/equipment/${walletAddress}`);
    if (!response.ok) {
      throw new Error('Failed to fetch owned equipments');
    }
    const data = await response.json();
    setOwnedEquipments(data);
  };

  const fetchBalance = async () => {
    const response = await fetch(`/api/player/${walletAddress}/balance`);
    if (!response.ok) {
      throw new Error('Failed to fetch balance');
    }
    const data = await response.json();
    setBalance(data);
  };

  const buyEquipment = async (equipmentId) => {
    try {
      setIsLoading(true);
      setErrorMessage('');
      
      const response = await fetch('/api/equipment/buy', {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify({ 
          walletAddress, 
          equipmentId 
        })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData || 'Failed to buy equipment');
      }
      
      await fetchData();
    } catch (error) {
      setErrorMessage(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  const upgradeEquipment = async (equipmentId) => {
    try {
      setIsLoading(true);
      setErrorMessage('');
      
      const response = await fetch('/api/equipment/upgrade', {
        method: 'POST',
        headers: { 
          'Content-Type': 'application/json',
          'Accept': 'application/json'
        },
        body: JSON.stringify({ 
          walletAddress, 
          equipmentId 
        })
      });
      
      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData || 'Failed to upgrade equipment');
      }
      
      await fetchData();
    } catch (error) {
      setErrorMessage(error.message);
    } finally {
      setIsLoading(false);
    }
  };

  const calculateUpgradeCost = (level) => {
    return (level * 0.1).toFixed(2);
  };

  return (
    <div className={`${styles.hocContainer} bg-siteblack`}>
      {isLoading && <GameLoad />}
      
      <div className="flex-1 overflow-y-auto">
        <div className="max-w-6xl mx-auto px-4 py-6">
          {/* Header */}
          <div className={`sticky top-0 bg-siteblack pt-4 pb-6 z-20 border-b border-siteViolet`}>
            <h2 className={styles.headText}>Equipment Shop</h2>
            <p className={styles.normalText}>
              Balance: {balance} AVAX
            </p>
            
            {errorMessage && (
              <div className={`${styles.alertWrapper} ${styles.failure} mt-4`}>
                {errorMessage}
              </div>
            )}
          </div>

          {/* Shop Equipment Section */}
          <div className="mt-8 mb-10">
            <h3 className={`${styles.joinHeadText} sticky top-40 bg-siteblack py-2 z-10`}>
              Available in Shop
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              {shopEquipments.map((equipment) => (
                <div 
                  key={equipment.id} 
                  className={`${styles.glassEffect} p-6 rounded-xl flex flex-col items-center`}
                >
                  <p className={styles.joinHeadText}>{equipment.name}</p>
                  <div className="space-y-2 text-center mb-4">
                    <p className={styles.normalText}>Level {equipment.level}</p>
                    <p className={styles.normalText}>Attack: {equipment.attack}</p>
                    <p className={styles.normalText}>Defense: {equipment.defense}</p>
                    <p className={styles.normalText}>Price: {equipment.price} AVAX</p>
                  </div>
                  
                  <CustomButton
                    title="Buy"
                    handleClick={() => buyEquipment(equipment.id)}
                    restStyles="w-full mt-2"
                  />
                </div>
              ))}
              {shopEquipments.length === 0 && (
                <p className={`${styles.normalText} text-center col-span-full`}>
                  No equipment available in shop
                </p>
              )}
            </div>
          </div>

          {/* Owned Equipment Section */}
          <div className="mb-10">
            <h3 className={`${styles.joinHeadText} sticky top-40 bg-siteblack py-2 z-10`}>
              Your Equipment
            </h3>
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
              {ownedEquipments.map((equipment) => (
                <div 
                  key={equipment.id} 
                  className={`${styles.glassEffect} p-6 rounded-xl flex flex-col items-center`}
                >
                  <p className={styles.joinHeadText}>{equipment.name}</p>
                  <div className="space-y-2 text-center mb-4">
                    <p className={styles.normalText}>Level {equipment.level}</p>
                    <p className={styles.normalText}>Attack: {equipment.attack}</p>
                    <p className={styles.normalText}>Defense: {equipment.defense}</p>
                  </div>
                  
                  <CustomButton
                    title={`Upgrade (${calculateUpgradeCost(equipment.level)} AVAX)`}
                    handleClick={() => upgradeEquipment(equipment.id)}
                    restStyles="w-full mt-2"
                  />
                </div>
              ))}
              {ownedEquipments.length === 0 && (
                <p className={`${styles.normalText} text-center col-span-full`}>
                  You don't own any equipment yet
                </p>
              )}
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};



export default PageHOC(
  EquipmentPage,
  <>Equipment Shop</>,
  <>Buy and upgrade your game equipment</>,
);
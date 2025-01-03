import { useEffect, useState } from 'react';
import { useGlobalContext } from '../context';
import styles from '../styles';

const Equipment = () => {
  const { walletAddress } = useGlobalContext();
  const [equipments, setEquipments] = useState([]);
  const [balance, setBalance] = useState(0);
  
  useEffect(() => {
    if (walletAddress) {
      fetchEquipments();
      fetchBalance();
    }
  }, [walletAddress]);

  const fetchEquipments = async () => {
    const response = await fetch(`/api/equipment/${walletAddress}`);
    const data = await response.json();
    setEquipments(data);
  };

  const fetchBalance = async () => {
    const response = await fetch(`/api/player/${walletAddress}/balance`);
    const data = await response.json();
    setBalance(data);
  };

  const buyEquipment = async (equipmentId) => {
    try {
      const response = await fetch('/api/equipment/buy', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ walletAddress, equipmentId })
      });
      
      if (response.ok) {
        await fetchEquipments();
        await fetchBalance();
      }
    } catch (error) {
      console.error('Error buying equipment:', error);
    }
  };

  const upgradeEquipment = async (equipmentId) => {
    try {
      const response = await fetch('/api/equipment/upgrade', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ walletAddress, equipmentId })
      });
      
      if (response.ok) {
        await fetchEquipments();
        await fetchBalance();
      }
    } catch (error) {
      console.error('Error upgrading equipment:', error);
    }
  };

  return (
    <div className={styles.flexBetween}>
      <div className={styles.gameContainer}>
        <h2 className={`${styles.headText} text-center`}>Equipment Shop</h2>
        <p className="font-poppins text-white text-lg text-center">Balance: {balance} AVAX</p>
        
        <div className="grid grid-cols-2 md:grid-cols-3 gap-4 mt-4">
          {equipments.map((equipment) => (
            <div key={equipment.id} className={`${styles.flexCenter} flex-col bg-[#292F3D] p-4 rounded-lg`}>
              <p className="font-poppins text-white text-xl">{equipment.name}</p>
              <p className="font-poppins text-white">Level {equipment.level}</p>
              <p className="font-poppins text-white">Attack: {equipment.attack}</p>
              <p className="font-poppins text-white">Defense: {equipment.defense}</p>
              <p className="font-poppins text-white">Price: {equipment.price} AVAX</p>
              
              {equipment.walletAddress === walletAddress ? (
                <button
                  className={`${styles.btn} mt-2`}
                  onClick={() => upgradeEquipment(equipment.id)}
                >
                  Upgrade
                </button>
              ) : (
                <button
                  className={`${styles.btn} mt-2`}
                  onClick={() => buyEquipment(equipment.id)}
                >
                  Buy
                </button>
              )}
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default Equipment;
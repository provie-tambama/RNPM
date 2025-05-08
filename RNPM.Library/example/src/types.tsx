import { type NativeStackNavigationProp } from '@react-navigation/native-stack';
import { type RouteProp } from '@react-navigation/native';

export type RootStackParamList = {
  Home: undefined;
  Details: undefined;
  Profile: undefined;
};

export type HomeScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Home'>;
export type DetailsScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Details'>;
export type ProfileScreenNavigationProp = NativeStackNavigationProp<RootStackParamList, 'Profile'>;

export type HomeScreenRouteProp = RouteProp<RootStackParamList, 'Home'>;
export type DetailsScreenRouteProp = RouteProp<RootStackParamList, 'Details'>;
export type ProfileScreenRouteProp = RouteProp<RootStackParamList, 'Profile'>;
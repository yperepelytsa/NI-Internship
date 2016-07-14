import { Injectable } from '@angular/core';
import { ITEMS } from './data-items';
@Injectable()
export class ItemService {
    
  getItems() {
    return ITEMS;
  }
}

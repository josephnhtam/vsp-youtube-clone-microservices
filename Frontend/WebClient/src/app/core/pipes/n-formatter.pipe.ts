import {Pipe, PipeTransform} from '@angular/core';

@Pipe({
  name: 'nFormatter',
})
export class NFormatterPipe implements PipeTransform {
  transform(value: number, digits: number): string {
    return this.nFormatter(value, digits);
  }

  nFormatter(num: number, digits: number) {
    const lookup = [
      { value: 1e3, symbol: 'k' },
      { value: 1e6, symbol: 'M' },
      { value: 1e9, symbol: 'G' },
      { value: 1e12, symbol: 'T' },
      { value: 1e15, symbol: 'P' },
      { value: 1e18, symbol: 'E' },
    ];

    let first: any = null;
    lookup.forEach((x) => {
      if (num >= x.value) {
        first = x;
      } else {
        return;
      }
    });

    if (first == null) {
      return num.toString();
    }

    const precision = Math.pow(10, digits);
    return (
      Math.round((num * precision) / first.value) / precision + first.symbol
    );
  }
}

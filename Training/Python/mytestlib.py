import os
import sys

stdin = sys.stdin.buffer
stdout = sys.stdout.buffer


def get_int_list():
    stdin.seek(0, os.SEEK_END)
    n = stdin.tell() // 4
    arr = [0] * n

    for i in range(n):
        arr[i] = int.from_bytes(stdin.read(4), byteorder='little')

    return arr


def write_int(i: int):
    stdout.write(i.to_bytes(4, byteorder='little'))


nums = get_int_list()

result = sum(nums)

write_int(result)
using System;

/// <summary>
/// Ringbuffer (generated)
/// </summary>
/// <typeparam name="T"></typeparam>
public class RingBuffer<T> where T : struct {
    private T[] buffer;
    private int currentTop;   // Points to the next element to pop
    private int writeIndex;   // Points to the next position to write
    private int count;

    public int Count => count;
    public int Capacity => buffer.Length;

    public RingBuffer(int initialCapacity = 8) {
        if (initialCapacity < 1) {
            throw new ArgumentException("Capacity must be greater than 0");
        }
        buffer = new T[initialCapacity];
        currentTop = 0;
        writeIndex = 0;
        count = 0;
    }

    public void Push(T item) {
        if (IsFull()) {
            Resize(buffer.Length * 2);
        }

        buffer[writeIndex] = item;
        writeIndex = (writeIndex + 1) % buffer.Length;
        count++;
    }

    public T Pop() {
        if (IsEmpty()) {
            throw new InvalidOperationException("Buffer is empty");
        }

        T item = buffer[currentTop];
        currentTop = (currentTop + 1) % buffer.Length;
        count--;
        return item;
    }

    public bool IsEmpty() {
        return count == 0;
    }

    public bool IsFull() {
        return count == buffer.Length;
    }

    private void Resize(int newCapacity) {
        T[] newBuffer = new T[newCapacity];
        for (int i = 0; i < count; i++) {
            newBuffer[i] = buffer[(currentTop + i) % buffer.Length];
        }
        buffer = newBuffer;
        currentTop = 0;
        writeIndex = count;
    }
}
